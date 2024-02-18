using UnityEngine;

namespace More.Client.Input
{
    public class InputState
    {
        public virtual void Start()
        {
            //Debug.Log("Input State Started.");
        }

        public virtual void Update()
        {
            //Debug.Log("Input State Updated.");
        }

        public virtual void Exit()
        {
            //Debug.Log("Input State Exit.");
        }

        public virtual void Move(Vector2 direction) { }
        public virtual void Aim(Vector2 delta) { }
        public virtual void JumpPress() { }
        public virtual void JumpRelease() { }
        public virtual void InteractPress() { }
        public virtual void InteractRelease() { }
        public virtual void ShootPress() { }
        public virtual void ShootRelease() { }
        public virtual void ReloadPress() { }
        public virtual void ReloadRelease() { }
    }
}