using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace More.Player
{
    public abstract class PlayerState
    {
        public virtual void Start()
        {
            Debug.Log("State Started");
        }

        public virtual void Update()
        {
            Debug.Log("State Updated");
        }

        public virtual void Exit()
        {
            Debug.Log("State Exit");
        }
    }


}

