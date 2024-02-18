using TMPro;
using UnityEngine;

namespace More.Client.UI
{
    public class IngameCanvasManager : MonoBehaviour
    {
        #region Components
        [field: SerializeField] public TMP_Text InteractionText {  get; private set; }
        #endregion

        public void OnInteractionHoverEnter(string tip)
        {
            InteractionText.text = tip;
            InteractionText.enabled = true;
        }

        public void OnInteractionHoverExit()
        {
            InteractionText.text = "";
            InteractionText.enabled = false;
        }
    }
}
