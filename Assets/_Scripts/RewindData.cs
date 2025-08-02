using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class RewindData : MonoBehaviour
{
    [Header("Rewind Settings")]
    [SerializeField] float rewindSpeed = 2f;
    [SerializeField] int maxRewindTime_InSeconds = 600;

    bool isRewinding = false;
    float rewindTimeBank = 10f;
    List<StateInTime> objectStatesInTime = new List<StateInTime>();

    [Header("References")]
    [SerializeField] TMP_Text textRewindTimeBank;

    Rigidbody rb;
    CharacterController_FirstPerson characterController;
    GameManager GM;

    void Start()
    {
        characterController = GetComponent<CharacterController_FirstPerson>();
        rb = GetComponent<Rigidbody>();
        GM = GameManager.GM;
        if (textRewindTimeBank == null)
        {
            Debug.LogError("TextRewindTimeBank is not assigned in the inspector.");
        }
        else
        {
            textRewindTimeBank.SetText($"Rewind Time Bank: {rewindTimeBank:F2}");
        }

        if (characterController == null)
        {
            Debug.LogError($"CharacterController_FirstPerson component not found on {gameObject.name}.");
        }
        if (rb == null)
        {
            Debug.LogError($"Rigidbody component not found on {gameObject.name}.");
        }
    }

    void Update()
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

    void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }

    }


    void Record()
    {
        // Limits the Max Rewind Time
        if (objectStatesInTime.Count > Mathf.RoundToInt(maxRewindTime_InSeconds / Time.fixedDeltaTime))
        {
            objectStatesInTime.RemoveAt(objectStatesInTime.Count - 1);
        }

        objectStatesInTime.Insert(0, new StateInTime(transform.position, transform.rotation));
    }


    void Rewind()
    {
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
        rewindTimeBank -= Time.fixedDeltaTime * rewindSpeed;
        rewindTimeBank = Mathf.Max(rewindTimeBank, 0f);
        textRewindTimeBank.text = $"Rewind Time Bank: {rewindTimeBank:F2}";
        GM.RewindGameTime(Time.fixedDeltaTime, rewindSpeed);

        transform.SetPositionAndRotation(objectStatesInTime[0].Position, objectStatesInTime[0].Rotation);
        objectStatesInTime.RemoveAt(0);

        for (int i = 0; i < rewindSpeed; i++)
        {
            if (objectStatesInTime.Count > 0)
            {
                objectStatesInTime.RemoveAt(0);
            }
        }

    }


    void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;

        characterController.enabled = false;


        Debug.Log("Rewind started");
    }

    void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;

        characterController.enabled = true;

        Debug.Log("Rewind stopped");
    }


    public void AddTimeBank(float amount)
    {
        rewindTimeBank += amount;
        textRewindTimeBank.text = $"Rewind Time Bank: {rewindTimeBank:F2}";
        // play feedback effects for the added time bank (eg. particle effect around the time, text increasing in size, time counting up)
    }
}
