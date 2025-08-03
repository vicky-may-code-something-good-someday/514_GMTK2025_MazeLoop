using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] float AddedTimeCharge = 10f;

    public void GetCollected()
    {
        GameObject interactor = gameObject.GetComponent<InteractableObj>().Interactor;
        if (interactor == null)
        {
            Debug.LogWarning("No interactor found. Cannot collect battery.");
            return;
        }

        interactor.GetComponent<RewindData>()?.AddTimeBank(AddedTimeCharge);


        //implement feedback effects of the battery being collected

        transform.parent.gameObject.SetActive(false);
        //Destroy(transform.parent.gameObject);
    }



}
