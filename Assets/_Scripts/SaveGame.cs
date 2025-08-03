using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    public Vector3 savedPlayerPosition = Vector3.zero;
    public Quaternion savedPlayerRotation = Quaternion.identity;


    public bool savedIsDeviceCollected = false;
    public float savedRewindTimeBank = 0f;
    public List<StateInTime> savedObjectStatesInTime = new List<StateInTime>();
    public float savedGameTime = 0f;

    //collected or not
    public Dictionary<Battery, bool> savedBatteries = new Dictionary<Battery, bool>();
    //unlocked or not
    public Dictionary<Door, bool> savedDoors = new Dictionary<Door, bool>();
    //activated or not
    public Dictionary<Button, bool> savedButtons = new Dictionary<Button, bool>();



}
