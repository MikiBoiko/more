using More.Player;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace More.Interactables
{
    [RequireComponent(typeof(Interactable))]
    public class Pickeable : NetworkBehaviour
    {
        #region Components
        public NetworkTransform NetworkTransform { get; private set; }
        public Interactable Interactable { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Collider Collider { get; private set; }
        #endregion

        #region Modify world interaction
        public void SetInteractable()
        {
            if(IsServer)
            {
                Rigidbody.isKinematic = false;
            }

            NetworkTransform.InLocalSpace = false;
            NetworkTransform.Interpolate = false;
            Collider.enabled = true;
        }

        public void SetUninteractable()
        {
            if(IsServer)
            {
                Rigidbody.isKinematic = true;
            }

            NetworkTransform.InLocalSpace = true;
            NetworkTransform.Interpolate = true;
            Collider.enabled = false;
        }
        #endregion

        #region Pick
        public void Pick(PlayerHands hands)
        {
            PickClientRpc(hands);
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void PickServerRpc(NetworkBehaviourReference handsReference)
        {
            PlayerHands hands;

            if(!handsReference.TryGet(out hands))
            {
                Debug.LogError("Hands reference is not found in the server.");
                return;
            }

            if (Vector3.Distance(transform.position, hands.transform.position) < PlayerHands.WATCH_DISTANCE)
            {
                Debug.LogError("Player is too far.");
                return;
            }

            PickClientRpc(handsReference);
        }

        [Rpc(SendTo.Everyone)]
        public void PickClientRpc(NetworkBehaviourReference handsReference)
        {
            PlayerHands hands;

            if (!handsReference.TryGet(out hands))
            {
                Debug.LogError("Hands reference is not found locally.");
                return;
            }

            hands.Pick(this);
            SetUninteractable();
        }
        #endregion

        #region Drop
        public void Drop()
        {

        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            #region Components
            NetworkTransform = GetComponent<NetworkTransform>();
            Interactable = GetComponent<Interactable>();
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            #endregion

            #region Interactable
            Interactable.onInteraction += (PlayerHands hands) => Pick(hands);
            #endregion
        }
        #endregion
    }
}
