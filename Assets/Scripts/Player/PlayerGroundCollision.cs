using System;
using UnityEngine;

namespace More.Player
{
    public class PlayerGroundCollision : MonoBehaviour
    {
        public delegate void CollisionEnter();
        public delegate void CollisionExit();

        public event CollisionEnter onCollisionEnter;
        public event CollisionExit onCollisionExit;

        private int _collisionCount;
        public bool IsActive => _collisionCount > 0;

        #region Testing
        [SerializeField] private Material _groundedMaterial, _notGroundedMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            #region Testing
            onCollisionEnter += () =>
            {
                _meshRenderer.material = _groundedMaterial;
            };

            onCollisionExit += () =>
            {
                _meshRenderer.material = _notGroundedMaterial;
            };
            #endregion
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                return;

            _collisionCount++;


            if (_collisionCount == 1)
                onCollisionEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                return;

            _collisionCount--;

            if (_collisionCount == 0)
                onCollisionExit?.Invoke();
        }
        #endregion
    }
}
