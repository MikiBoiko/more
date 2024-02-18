using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace More.Client.Input
{
    public class InputStateManager : PlayerInput
    {
        private InputActionMap _actionMap;

        public InputState State { private set; get; }

        public void SetState(InputState state)
        {
            State?.Exit();
            State = state;
            state.Start();
        }

        private void Awake()
        {
            SetState(
                new UnconnectedInputState(
                    GameObject.Find("Network")
                        .GetComponent<NetworkManager>()
                )
            );

            _actionMap = actions.FindActionMap("Game");
            _actionMap.Enable();

            _actionMap["Move"].performed += ctx => State.Move(ctx.ReadValue<Vector2>());
            _actionMap["Move"].canceled += ctx => State.Move(Vector2.zero);
            _actionMap["Aim"].performed += ctx => State.Aim(ctx.ReadValue<Vector2>());
            _actionMap["Aim"].canceled += ctx => State.Aim(Vector2.zero);
            _actionMap["Jump"].performed += ctx => State.JumpPress();
            _actionMap["Jump"].canceled += ctx => State.JumpRelease();
            _actionMap["Interact"].performed += ctx => State.InteractPress();
            _actionMap["Interact"].canceled += ctx => State.InteractRelease();
            _actionMap["Shoot"].performed += ctx => State.ShootPress();
            _actionMap["Shoot"].canceled += ctx => State.ShootRelease();
            _actionMap["Reload"].performed += ctx => State.ReloadPress();
            _actionMap["Reload"].canceled += ctx => State.ReloadRelease();
        }

        private void FixedUpdate()
        {
            State?.Update();
        }
    }
}

