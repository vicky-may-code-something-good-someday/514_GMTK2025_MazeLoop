using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class CheckForDevice : MonoBehaviour
{
    [SerializeField] DOTweenAnimation openDoorAnimation;

    public bool isUnlocked = false;
    bool isOpen = false;

    [Tooltip("This Door gets opened by every Button with the same ConnectionCode")]
    public int DoorConnectionCode;


    public void OpenDoor()
    {
        if (isOpen)
        {
            //Debug.Log("Door is already open.");
            return;
        }

        if (!isUnlocked)
        {
            //play door is locked sound
            //Debug.LogWarning("Door is locked.");
            return;
        }

        isOpen = true;
        Debug.Log($"Opening door with ConnectionCode: {DoorConnectionCode}");
        openDoorAnimation.DOPlay();

        GetComponent<Collider>().enabled = false; // Disable trigger collider
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
    }

}
