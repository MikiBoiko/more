using More.Client.Input;
using More.Client.Camera;
using More.Client.UI;
using UnityEngine;

namespace More.Client
{
    public class LocalManager : MonoBehaviour
    {
        private static LocalManager _;
        public static LocalManager Current => _;

        [field: SerializeField] 
        public InputStateManager InputState { get; private set; }
        
        [field: SerializeField] 
        public CameraManager Camera { get; private set; }

        [field: SerializeField]
        public IngameCanvasManager UI { get; private set; }

        private void Awake()
        {
            if(_ != null)
            {
                Debug.LogError("Two Local instances in scene.");
                Destroy(gameObject);
                return;
            }

            _ = this;
        }

        public static void SetLocalPlayer(PlayerManager playerManager)
        {
            _.InputState.SetState(new IngameInputState(playerManager));
            _.Camera.transform.SetParent(playerManager.Motor.Head);
            _.Camera.transform.localPosition = Vector3.zero;
        }
    }
}

