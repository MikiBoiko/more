using UnityEngine;
using Unity.Netcode;

namespace More.Test
{
    public class NetworkSpawner : NetworkBehaviour
    {
        [field: SerializeField] public GameObject Prefab { get; set; }

        private void Start()
        {
            if(!IsServer)
                NetworkManager.OnServerStarted += Spawn;
        }

        public void Spawn()
        {
            if (Prefab == null)
            {
                Debug.LogWarning("null prefab with NetworkObject to spawn.");
                return;
            }
            GameObject prefab = Instantiate(Prefab);
            NetworkObject networkObject = prefab.GetComponent<NetworkObject>();
            networkObject.Spawn(destroyWithScene: true);
        }
    }
}
