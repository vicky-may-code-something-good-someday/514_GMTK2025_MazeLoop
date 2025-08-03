using UnityEngine;
using UnityEngine.Events;

public class InteractableObj : MonoBehaviour
{
    public bool isInteractable = true;
    //The hitbox would still be there meaning the UI for Press [E] to interact would still be visible


    [SerializeField] UnityEvent interaction;

    public GameObject Interactor { get; private set; }


    public void GetInteracted(GameObject interactor)
    {
        if (!isInteractable)
        {
            return;
        }

        Interactor = interactor;

        if (interaction != null)
        {
            interaction.Invoke();
        }
        else
        {
            Debug.LogWarning($"No interaction defined for {gameObject.name}. Please assign an interaction in the inspector.");
        }
    }
}
