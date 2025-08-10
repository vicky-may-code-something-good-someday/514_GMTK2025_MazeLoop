using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation openDoorAnimation;

    [Header("Door Settings")]
    public bool isUnlocked = false;
    private bool isOpen = false;

    [Tooltip("This Door gets opened by every Button with the same ConnectionCode")]
    public int DoorConnectionCode;

    [Header("Door Events")]
    public UnityEvent OnDoorOpened;
    public UnityEvent OnDoorClosed;
    public UnityEvent OnDoorLocked;

    public void OpenDoor()
    {
        if (isOpen)
        {
            return;
        }

        if (!isUnlocked)
        {
            // Play door locked event
            OnDoorLocked?.Invoke();
            return;
        }

        isOpen = true;
        Debug.Log($"Opening door with ConnectionCode: {DoorConnectionCode}");
        openDoorAnimation.DOPlay();
        GetComponent<Collider>().enabled = false;

        // Invoke open event
        OnDoorOpened?.Invoke();
    }

    public void CloseDoor()
    {
        if (!isOpen)
        {
            return;
        }

        isOpen = false;
        Debug.Log($"Closing door with ConnectionCode: {DoorConnectionCode}");
        openDoorAnimation.DOPlayBackwards();
        GetComponent<Collider>().enabled = true;

        // Invoke close event
        OnDoorClosed?.Invoke();
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
    }
}