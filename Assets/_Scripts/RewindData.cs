using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.WSA;

public class RewindData : MonoBehaviour
{
    [Header("Rewind Settings")]
    [SerializeField] int rewindSpeed = 2;
    [SerializeField] int maxRewindedTime_inSeconds = 600;
    public bool isDeviceCollected = false;
    [SerializeField] bool isRecordingRewindingPossible = true;

    bool isRewinding = false;
    public float rewindTimeBank = 10f;
    public List<StateInTime> objectStatesInTime = new List<StateInTime>();

    [Header("References")]
    [SerializeField] TMP_Text textUI_TimeBank;

    Rigidbody rb;
    CharacterController_FirstPerson characterController;
    GameManager GM;

    void Start()
    {
        characterController = GetComponent<CharacterController_FirstPerson>();
        rb = GetComponent<Rigidbody>();

        if (characterController == null)
        {
            Debug.LogError($"CharacterController_FirstPerson component not found on {gameObject.name}.");
        }
        if (rb == null)
        {
            Debug.LogError($"Rigidbody component not found on {gameObject.name}.");
        }

        GM = GameManager.GM;

        if (textUI_TimeBank == null)
        {
            Debug.LogError("TextRewindTimeBank is not assigned in the inspector.");
        }
        else
        {
            textUI_TimeBank.gameObject.SetActive(false);
            textUI_TimeBank.SetText($"Rewind Time Bank: {rewindTimeBank:F2}");
        }


        if (isDeviceCollected)
        {
            ActivateRewindMechanic();
        }
    }

    void Update()
    {
        if (isDeviceCollected)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartRewind();
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                StopRewind();
            }
        }
    }

    void FixedUpdate()
    {
        if (isRewinding && isDeviceCollected && isRecordingRewindingPossible)
        {
            Rewind();
        }
        else if (isRecordingRewindingPossible)
        {
            Record();
        }

    }


    void Record()
    {
        // Limits the Max Rewind Time
        if (objectStatesInTime.Count > Mathf.RoundToInt(maxRewindedTime_inSeconds / Time.fixedDeltaTime))
        {
            objectStatesInTime.RemoveAt(objectStatesInTime.Count - 1);
        }

        objectStatesInTime.Insert(0, new StateInTime(transform.position, transform.rotation));
    }


    void Rewind()
    {
        // Remove states based on rewind speed
        // Problem: only works with integer rewind speeds
        for (int i = 0; i < rewindSpeed; i++)
        {
            if (objectStatesInTime.Count > 0)
            {
                objectStatesInTime.RemoveAt(0);
            }
        }

        if (objectStatesInTime.Count <= 0)
        {
            Debug.LogWarning("No positions to rewind to.");
            StopRewind();
            return;
        }

        if (rewindTimeBank <= 0f)
        {
            StopRewind();
            return;
        }

        transform.SetPositionAndRotation(objectStatesInTime[0].Position, objectStatesInTime[0].Rotation);


        rewindTimeBank -= Time.fixedDeltaTime * rewindSpeed;
        rewindTimeBank = Mathf.Max(rewindTimeBank, 0f);
        textUI_TimeBank.text = $"Rewind Time Bank: {rewindTimeBank:F2}";

        GM.RewindGameTime(Time.fixedDeltaTime, rewindSpeed);

    }


    void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;

        characterController.enabled = false;

        GM.SetPauseGameTime(true);

        //Debug.Log("Rewind started");
    }

    void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;

        characterController.enabled = true;

        GM.SetPauseGameTime(false);

        //Debug.Log("Rewind stopped");
    }


    public void AddTimeBank(float amount)
    {
        rewindTimeBank += amount;
        textUI_TimeBank.text = $"Rewind Time Bank: {rewindTimeBank:F2}";
        // play feedback effects for the added time bank (eg. particle effect around the time, text increasing in size, time counting up)
    }

    public void FreezeRewindMechanic()
    {
        if (!isRewinding)
        {
            isRecordingRewindingPossible = false;
            GM.SetPauseGameTime(true);
        }
    }

    public void UnfreezeRewindMechanic()
    {
        isRecordingRewindingPossible = true;
        GM.SetPauseGameTime(false);
    }

    public void ActivateRewindMechanic()
    {
        isDeviceCollected = true;
        textUI_TimeBank.gameObject.SetActive(true);
    }

}
