using UnityEngine;

namespace More.Client.Camera
{
    public class CameraManager : MonoBehaviour
    {
        #region Components
        public UnityEngine.Camera Camera { get; private set; }
        #endregion

        #region UnityEngine
        private void Awake()
        {
            #region Components
            Camera = GetComponent<UnityEngine.Camera>();
            #endregion
        }
        #endregion
    }
}