using UnityEngine;

public class RewindDevice : MonoBehaviour
{

    public void GetCollected()
    {
        GameObject interactor = gameObject.GetComponent<InteractableObj>().Interactor;
        if (interactor == null)
        {
            Debug.LogWarning("No interactor found. Cannot collect rewind device.");
            return;
        }

        interactor.GetComponent<RewindData>()?.ActivateRewindMechanic();

        //implement feedback effects of the rewind device being collected

        transform.parent.gameObject.SetActive(false);
        //Destroy(transform.parent.gameObject);
    }
}
