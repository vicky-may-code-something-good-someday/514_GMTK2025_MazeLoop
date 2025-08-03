using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] float BatteryTimeCharge = 10f;

    public void GetCollected()
    {
        GameObject interactor = gameObject.GetComponent<InteractableObj>().Interactor;
        if (interactor == null)
        {
            Debug.LogWarning("No interactor found. Cannot collect battery.");
            return;
        }

        interactor.GetComponent<RewindData>()?.AddTimeBank(BatteryTimeCharge);


        //implement feedback effects of the battery being collected

        Destroy(transform.parent.gameObject); // Destroy the battery object after collection
    }



}
