using UnityEngine;
using Unity.Netcode;
using More.Player;
using More.Client;

namespace More
{
    public class PlayerManager : NetworkBehaviour
    {
        #region Components
        [field: Header("Components")]
        [field: SerializeField] public PlayerMotor Motor { get; private set; }
        [field: SerializeField] public PlayerHands Hands { get; private set; }
        #endregion

        #region Player state
        public PlayerState PlayerState { get; private set; }

        public void SetState(PlayerState state)
        {
            PlayerState?.Exit();
            PlayerState = state;
            PlayerState.Start();
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            #region GetComponents
            Motor = GetComponent<PlayerMotor>();
            #endregion
        }

        private void Start()
        {
            #region Local player
            if (IsOwner)
            {
                LocalManager.SetLocalPlayer(this);
            }
            #endregion
        }
        #endregion
    }
}
