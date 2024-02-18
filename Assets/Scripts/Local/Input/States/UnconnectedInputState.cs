using Unity.Netcode;

namespace More.Client.Input
{
    public class UnconnectedInputState : InputState
    {
        private NetworkManager _networkManager;

        public UnconnectedInputState(NetworkManager networkManager) : base()
        {
            _networkManager = networkManager;
        }

        public override void JumpPress() 
        {
            _networkManager.StartHost();
        }

        public override void ReloadPress()
        {
            _networkManager.StartClient();
        }
    }
}