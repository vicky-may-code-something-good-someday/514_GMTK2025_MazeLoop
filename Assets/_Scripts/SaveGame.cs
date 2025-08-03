using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    public Vector3 savedPlayerPosition;
    public Quaternion savedPlayerRotation;


    public bool savedIsDeviceCollected;
    public float savedRewindTimeBank;
    public List<StateInTime> savedObjectStatesInTime = new List<StateInTime>();
    public float savedGameTime;

    //collected or not
    public Dictionary<Battery, bool> savedBatteries = new Dictionary<Battery, bool>();
    //unlocked or not
    public Dictionary<Door, bool> savedDoors = new Dictionary<Door, bool>();
    //activated or not
    public Dictionary<Button, bool> savedButtons = new Dictionary<Button, bool>();



}
