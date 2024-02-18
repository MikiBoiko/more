using Unity.Netcode;
using UnityEngine;

namespace More
{
    public class Field : NetworkBehaviour
    {
        public delegate void OnFieldEnter();
        public delegate void OnFieldLeave();

        public event OnFieldEnter onFieldEnter;
        public event OnFieldLeave onFieldLeave;

        [Rpc(SendTo.Everyone)]
        public void OnFieldEnterEveryoneRpc()
        {
            onFieldEnter?.Invoke();
        }

        [Rpc(SendTo.Everyone)]
        public void OnFieldLeaveEveryoneRpc()
        {
            onFieldLeave?.Invoke();
        }

        #region MonoBehaviour
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            OnFieldEnterEveryoneRpc();
        }
        private void OnTriggerExit(Collider other)
        {
            if (!IsServer)
                return;

            OnFieldLeaveEveryoneRpc();
        }
        #endregion
    }
}