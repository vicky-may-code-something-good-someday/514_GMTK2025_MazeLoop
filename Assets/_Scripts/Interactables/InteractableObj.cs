using UnityEngine;
using UnityEngine.Events;

public class InteractableObj : MonoBehaviour
{

    [SerializeField] UnityEvent interaction;

    public bool isInteractable = true;
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
