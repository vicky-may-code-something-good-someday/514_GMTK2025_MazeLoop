using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class CheckForDevice : MonoBehaviour
{
    public RewindData rewindData;


    [Tooltip("If false, the door has to be manually opened and will only be unlocked")]
    public bool openDoorDirectly = true;

    [Tooltip("This Button opens every Door with the same ConnectionCode")]
    public int ButtonConnectionCode;

    public bool isActivated = false;

    public void UnlockConnectedDoor()
    {
        if (rewindData.isDeviceCollected)
        {

            if (isActivated)
            {
                //Debug.LogWarning("Button is already activated.");
                return;
            }


            isActivated = true;

            if (openDoorDirectly)
            {
                DoorManager.Instance.UnlockDoorsByID(ButtonConnectionCode, openDoorDirectly);
            }

            GetComponent<Collider>().enabled = false; // Disable trigger collider
        }
    }
}