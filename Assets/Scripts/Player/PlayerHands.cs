using More.Client;
using More.Client.UI;
using More.Interactables;
using Unity.Netcode;
using UnityEngine;
namespace More.Player
{
    public class PlayerHands : NetworkBehaviour
    {
        #region Attributes
        public static readonly float WATCH_DISTANCE = 3f;
        [SerializeField] private LayerMask _intecactionMask;
        #endregion

        #region Components
        [field: SerializeField] public IngameCanvasManager CanvasManager { get; private set; }
        [field: SerializeField] public Transform Transfrom { get; private set; }
        [field: SerializeField] public Transform Head { get; private set; }
        [field: SerializeField] public Interactable Interactable { get; private set; }
        [field: SerializeField] public Pickeable Pickeable { get; private set; }
        public bool HasPickeable => Pickeable != null;
        #endregion

        #region Network variables
        private NetworkVariable<NetworkBehaviourReference> PickeableReference = new();
        #endregion

        #region Watch and interact
        public void Watch()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(Head.position, Head.forward, out hit, WATCH_DISTANCE, _intecactionMask);
            Debug.DrawRay(Head.position, Head.forward * WATCH_DISTANCE, hasHit ? Color.green : Color.red);

            if (!hasHit)
            {
                CanvasManager.OnInteractionHoverExit();
                Interactable = null;
                return;
            }

            Interactable interactable = hit.collider.GetComponent<Interactable>();
            Interactable = interactable;
            CanvasManager.OnInteractionHoverEnter(interactable.TextTip);
            Debug.DrawRay(hit.point, hit.normal, Color.yellow);
        }

        public void Interact()
        {
            Debug.Log("Interaction...");
            Interactable.Interact(this);
        }
        #endregion

        #region Pick and drop item
        public void Pick(Pickeable pickeable)
        {
            Debug.Log("Picking item");

            if(IsServer)
            {
                PickeableReference.Value = new NetworkBehaviourReference(pickeable);
                pickeable.transform.parent = NetworkObject.transform;
            }

            Pickeable = pickeable;
        }
        #endregion

        #region MonoBehaviour
        public override void OnDestroy()
        {
            base.OnDestroy();

            #region Pickeable
            if(IsServer)
                Pickeable?.Drop();
            #endregion
        }

        private void Awake()
        {
            #region Components
            Transfrom = transform;
            CanvasManager = LocalManager.Current.UI;
            #endregion
        }

        private void Start()
        {
            #region Pickeable 
            Pickeable = (Pickeable)PickeableReference.Value;
            if (Pickeable != null)
            {
                Pickeable.SetUninteractable();
            }
            #endregion
        }

        private void FixedUpdate()
        {
            #region Watch
            if (IsOwner)
            {
                Watch();
            }
            #endregion
        }

        private void Update()
        {
            #region Pickeable
            if(Pickeable != null)
            {
                Transform pickeableTransform = Pickeable.transform;

                pickeableTransform.position = Transfrom.position;
                pickeableTransform.forward = Head.forward;
            }
            #endregion            
        }
        #endregion
    }
}
