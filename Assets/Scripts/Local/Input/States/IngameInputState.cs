using More.Player;
using UnityEngine;

namespace More.Client.Input
{
    public class IngameInputState : InputState
    {
        private bool _isCharging = false;
        private float _minCharge = 0.75f;
        private float _charge;
        private float _chargeSpeed = 0.5f;

        #region Components
        private PlayerManager _playerManager;
        private PlayerMotor _motor;
        private PlayerHands _hands;
        #endregion

        public IngameInputState(PlayerManager playerManager) : base()
        {
            _playerManager = playerManager;
            _motor = _playerManager.Motor;
            _hands = _playerManager.Hands;
        }

        #region InputState
        public override void Update()
        {
            #region Charge jump
            if (_isCharging)
            {
                _charge = Mathf.Clamp(_charge + (_chargeSpeed * Time.fixedDeltaTime), _minCharge, 1f);
            }
            #endregion
        }

        public override void Move(Vector2 direction) 
        {
            _motor.SetMoveDirection(direction);
        }

        public override void Aim(Vector2 delta) 
        {
            _motor.Aim(delta);
        }

        public override void JumpPress() 
        {
            _isCharging = true;
        }
        
        public override void JumpRelease() 
        {
            _isCharging = false;
            _motor.Jump(_charge);
            _charge = _minCharge;
        }

        public override void InteractPress()
        {
            _hands.Interact();
        }

        public override void ShootPress() { }
        public override void ShootRelease() { }
        #endregion
    }
}