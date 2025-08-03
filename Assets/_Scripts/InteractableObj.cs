using UnityEngine;
using UnityEngine.Events;

public class InteractableObj : MonoBehaviour
{

    [SerializeField] UnityEvent interaction;

    public void GetInteracted()
    {
        if (interaction != null)
        {
            interaction.Invoke();
        }
    }
}
