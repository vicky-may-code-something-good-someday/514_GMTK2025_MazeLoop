using UnityEngine;

public class FreezeTimerArea : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"FreezeTimerArea: {other.name} entered the trigger.");
        if (other.CompareTag("Player"))
        {
            RewindData rewindData = other.GetComponent<RewindData>();
            if (rewindData != null)
            {
                rewindData.FreezeRewindMechanic();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"FreezeTimerArea: {other.name} exited the trigger.");
        if (other.CompareTag("Player"))
        {
            RewindData rewindData = other.GetComponent<RewindData>();
            if (rewindData != null)
            {
                rewindData.UnfreezeRewindMechanic();
            }
        }
    }
}
