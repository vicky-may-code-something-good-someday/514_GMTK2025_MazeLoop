using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] DOTweenAnimation openDoorAnimation;

    [SerializeField] bool isLocked = false;
    bool isOpen = false;

    [Tooltip("This Door gets opened by every Button with the same ConnectionCode")]
    public int DoorConnectionCode;


    public void OpenDoor()
    {
        if (isLocked)
        {
            //play door is locked sound
            //Debug.LogWarning("Door is locked.");
            return;
        }

        if (isOpen)
        {
            //Debug.Log("Door is already open.");
            return;
        }

        isOpen = true;

        openDoorAnimation.DOPlay();

        GetComponent<Collider>().enabled = false; // Disable trigger collider
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }

}
