using UnityEngine;
using Unity.Netcode;
using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace More.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMotor : NetworkBehaviour
    {
        private static readonly float MAX_AIM_ANGLE = 80f;
        private static readonly float MAX_STAMINA = 100f;
        private static readonly float MIN_MOVE_MAGNITUDE = 0.01f;

        #region Events
        public delegate void OnStaminaChangeEvent(float stamina);
        public event OnStaminaChangeEvent OnStaminaChange;
        #endregion

        #region Components
        [SerializeField] private Rigidbody _rigidbody;
        [field: SerializeField] public Transform Body { get; private set; }
        [field: SerializeField] public Transform Head { get; private set; }
        [field: SerializeField] public PlayerGroundCollision GroundCollision { get; private set; }
        #endregion

        #region State
        [System.Serializable]
        public class State
        {
            [field: SerializeField] public Vector2 MoveDirection { get; set; }
            [field: SerializeField] public float MoveTopSpeed { get; set; }
            [field: SerializeField] public float MoveAcceleration { get; set; }
            [field: SerializeField] public AnimationCurve MoveAccelerationCurve { get; set; }
            [field: Range(0f, 1f)][field: SerializeField] public float MoveAirSpeedDecay { get; set; }
            [field: Range(0f, 1f)] [field: SerializeField] public float MoveGroundedSpeedDecay {  get; set; }
            [field: SerializeField] public float JumpForce {  get; set; }
            [field: SerializeField] public Vector2 AimDirection { get; set; }
            [field: SerializeField] public float AimSpeed { get; set; }
            [field: SerializeField] public float Stamina {  get; set; }
        }

        [field: SerializeField] public State CurrentState { get; private set; }
        #endregion

        #region Ground collision
        public bool Grounded => GroundCollision.IsActive;

        private void OnGroundedEnter() { }

        private void OnGroundedExit() { }
        #endregion

        #region Move
        private void Move()
        {   
            Vector2 moveDirection = CurrentState.MoveDirection;

            if (moveDirection.magnitude < MIN_MOVE_MAGNITUDE && !_justJumped && Grounded)
            {
                _rigidbody.velocity *= CurrentState.MoveGroundedSpeedDecay;
            }

            float turnAngle = -CurrentState.AimDirection.x * Mathf.Deg2Rad;
            float fwdVelAngle = Vector3.Angle(_rigidbody.velocity, Body.rotation * new Vector3(CurrentState.MoveDirection.x, 0, CurrentState.MoveDirection.y));
            float clampedMag = 1 - Mathf.Clamp01(fwdVelAngle / 60);
            float moveAccelerationEvaluation = Mathf.Clamp01((_rigidbody.velocity / CurrentState.MoveTopSpeed).magnitude * clampedMag);

            Vector3 moveForce = new Vector3(
                moveDirection.x * Mathf.Cos(turnAngle) - moveDirection.y * Mathf.Sin(turnAngle),
                0,
                moveDirection.x * Mathf.Sin(turnAngle) + moveDirection.y * Mathf.Cos(turnAngle)
            ) * CurrentState.MoveAccelerationCurve.Evaluate(1 - moveAccelerationEvaluation) * CurrentState.MoveAcceleration;

            if(!Grounded)
            {
                moveForce *= CurrentState.MoveAirSpeedDecay;
            }


            _rigidbody.AddForce(moveForce);
        }

        public void SetMoveDirection(Vector2 direction)
        {
            UpdateMoveDirectionServerRpc(direction);
        }

        [Rpc(SendTo.Server, RequireOwnership = true)]
        public void UpdateMoveDirectionServerRpc(Vector2 direction)
        {
            Vector2 directionNormalized = Vector2.ClampMagnitude(direction, 1f);
            UpdateMoveDirectionClientRpc(directionNormalized);
        }

        [Rpc(SendTo.Everyone)]
        public void UpdateMoveDirectionClientRpc(Vector2 direction)
        {
            CurrentState.MoveDirection = direction;
        }
        #endregion

        #region Jump
        private bool _justJumped = false;

        public void Jump(float charge)
        {
            JumpServerRpc(charge);
        }

        [Rpc(SendTo.Server, RequireOwnership = true)]
        public void JumpServerRpc(float charge)
        {
            float clampedCharge = Mathf.Clamp01(charge);

            if(Grounded && !_justJumped)
                JumpEveryoneRpc(clampedCharge);
        }

        [Rpc(SendTo.Everyone)]
        public void JumpEveryoneRpc(float charge)
        {
            _rigidbody.AddRelativeForce(Vector3.up * CurrentState.JumpForce * charge);
            StartCoroutine(IJustJumped());
        }

        private IEnumerator IJustJumped()
        {
            _justJumped = true;
            yield return new WaitForSeconds(0.5f);
            _justJumped = false;
        }
        #endregion

        #region Aim
        public void Aim(Vector2 delta)
        {
            float zEulerRotation = delta.x * CurrentState.AimSpeed;
            float zEulerAngle = CurrentState.AimDirection.x + zEulerRotation;
            zEulerAngle %= 360f;

            float xEulerRotation = delta.y * CurrentState.AimSpeed;
            float xEulerAngle = CurrentState.AimDirection.y + xEulerRotation;
            if (xEulerAngle > 180f)
                xEulerAngle -= 360f;
            xEulerAngle = Mathf.Clamp(xEulerAngle, -MAX_AIM_ANGLE, MAX_AIM_ANGLE);

            Vector2 direction = new Vector2(zEulerAngle, xEulerAngle);

            AimServerRpc(direction);
            AimTowards(direction);
        }

        public void AimTowards(Vector2 direction)
        {
            Body.localRotation = Quaternion.Euler(0, direction.x, 0);
            Head.localRotation = Quaternion.Euler(-direction.y, 0, 0);
            CurrentState.AimDirection = direction;
        }

        [Rpc(SendTo.Server, RequireOwnership = true)]
        public void AimServerRpc(Vector2 direction)
        {
            AimNotOwnerRpc(direction);
        }

        [Rpc(SendTo.NotOwner)]
        public void AimNotOwnerRpc(Vector2 direction)
        {
            CurrentState.AimDirection = direction;
        }
        #endregion

        #region Stamina
        [Rpc(SendTo.Everyone)]
        public void WasteStaminaRpc(float staminaToWaste)
        {
            float newStamina = Mathf.Clamp(CurrentState.Stamina - staminaToWaste, 0f, MAX_STAMINA);
            OnStaminaChange.Invoke(newStamina);
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            #region Get components
            _rigidbody = GetComponent<Rigidbody>();
            #endregion

            #region Ground collision
            GroundCollision.onCollisionEnter += OnGroundedEnter;
            GroundCollision.onCollisionExit += OnGroundedExit;
            #endregion
        }

        private void FixedUpdate()
        {
            #region Move player
            Move();
            #endregion

            #region Aim player
            if (!IsOwner)
            {
                AimTowards(CurrentState.AimDirection);
            }
            #endregion
        }
        #endregion
    }
}
