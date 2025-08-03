using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    //Singleton
    public static DoorManager Instance { get; private set; }

    public List<Door> doors;
    public List<Button> buttons;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        doors = new List<Door>(FindObjectsByType<Door>(FindObjectsSortMode.None));
        buttons = new List<Button>(FindObjectsByType<Button>(FindObjectsSortMode.None));
    }

    public void UnlockDoorsByID(float doorID, bool openDoor = false)
    {
        foreach (Door door in doors)
        {
            if (door.DoorConnectionCode == doorID)
            {
                door.UnlockDoor();

                if (openDoor)
                {
                    door.OpenDoor();
                }
            }
        }
    }

}
