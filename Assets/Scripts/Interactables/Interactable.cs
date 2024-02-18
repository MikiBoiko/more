using More.Player;
using UnityEngine;

namespace More.Interactables
{
    public class Interactable : MonoBehaviour
    {
        [field: SerializeField] public string TextTip { protected set; get; }
        public delegate void InteractableDelegate(PlayerHands hands);
        public event InteractableDelegate onInteraction;

        public void Interact(PlayerHands playerHands)
        {
            onInteraction?.Invoke(playerHands);
            OnInteract(playerHands);
        }
        protected virtual void OnInteract(PlayerHands playerHands) 
        {
            Debug.Log("Interacted with item");
        }
    }
}
