using UnityEngine;

public class RewindDevice : MonoBehaviour
{

    public void GetCollected()
    {
        GameObject interactor = gameObject.GetComponent<InteractableObj>().Interactor;
        if (interactor == null)
        {
            Debug.LogWarning("No interactor found. Cannot collect battery.");
            return;
        }

        interactor.GetComponent<RewindData>()?.ActivateRewindMechanic();

        //implement feedback effects of the rewind device being collected

        Destroy(transform.parent.gameObject); // Destroy the rewind device object after collection
    }
}
