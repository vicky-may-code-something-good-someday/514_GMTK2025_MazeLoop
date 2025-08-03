using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SaveGame currentSaveGame;

    // Singleton
    public static GameManager GM { get; private set; }

    void Awake()
    {
        if (GM == null)
        {
            GM = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentSaveGame = GetComponent<SaveGame>();

    }

    [Header("Interactables")]
    [HideInInspector] public List<Battery> batteriesList = new List<Battery>();
    [HideInInspector] public List<Door> doorsList = new List<Door>();
    [HideInInspector] public List<Button> buttonsList = new List<Button>();

    [HideInInspector] public Dictionary<Battery, bool> batteriesDictionary = new Dictionary<Battery, bool>();
    [HideInInspector] public Dictionary<Door, bool> doorsDictionary = new Dictionary<Door, bool>();
    [HideInInspector] public Dictionary<Button, bool> buttonsDictionary = new Dictionary<Button, bool>();

    public RewindData rewindData;
    public CharacterController_FirstPerson characterController;
    public GameObject RewindDevice;

    [SerializeField] TMP_Text text_gameTime;
    //float rewindedTime = 0f;
    public float gameTime = 0f;
    bool pauseGameTime = false;


    void Start()
    {
        UpdateInteractablesList();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadSaveFile();
        }


        if (!pauseGameTime)
        {
            gameTime += Time.deltaTime;
        }

        text_gameTime.text = gameTime.ToString("F2");
    }

    public void RewindGameTime(float amount, float rewindSpeed = 1f)
    {
        gameTime -= amount * rewindSpeed;
        //rewindedTime += amount * rewindSpeed;
    }

    public void SetPauseGameTime(bool state)
    {
        pauseGameTime = state;
    }

    public void UpdateInteractablesList()
    {
        batteriesList = new List<Battery>(FindObjectsByType<Battery>(FindObjectsSortMode.None));
        foreach (var battery in batteriesList)
        {
            batteriesDictionary[battery] = battery.isCollected; // false means is untouched
        }

        doorsList = new List<Door>(FindObjectsByType<Door>(FindObjectsSortMode.None));
        foreach (var door in doorsList)
        {
            doorsDictionary[door] = door.isUnlocked; // true means is untouched
        }

        buttonsList = new List<Button>(FindObjectsByType<Button>(FindObjectsSortMode.None));
        foreach (var button in buttonsList)
        {
            buttonsDictionary[button] = button.isActivated; // false means is untouched
        }

    }




    public void LoadSaveFile()
    {
        if (currentSaveGame == null)
        {
            Debug.LogWarning("No save file found. Cannot load game state.");
            return;
            //restart the game?
        }

        //Reset Movement and Rewind Mechanics
        characterController.FreezeMovement();
        characterController.UnfreezeMovement();

        rewindData.StopRewind();
        rewindData.UnfreezeRewindMechanic();

        //Set to Checkpoint Position/Rotation
        CharacterController cc = rewindData.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false; // disable before changing position
        }

        rewindData.transform.position = currentSaveGame.savedPlayerPosition + new Vector3(0, 1f, 0); // Adjust Y position to avoid ground clipping
        rewindData.transform.rotation = currentSaveGame.savedPlayerRotation;

        if (cc != null)
        {
            cc.enabled = true; // re-enable after changing position
        }

        //Set TimerData to Saved State
        rewindData.isDeviceCollected = currentSaveGame.savedIsDeviceCollected;

        if (RewindDevice != null)
        {
            RewindDevice.SetActive(!currentSaveGame.savedIsDeviceCollected);
            //bug that ui is not updated if the device is not collected anymore
        }

        rewindData.rewindTimeBank = currentSaveGame.savedRewindTimeBank;
        rewindData.objectStatesInTime.Clear();
        foreach (var state in currentSaveGame.savedObjectStatesInTime)
        {
            rewindData.objectStatesInTime.Add(state);
        }

        gameTime = currentSaveGame.savedGameTime;

        //Reset Timer UI
        rewindData.SetUI_TimeBank();
        text_gameTime.text = gameTime.ToString("F2");


        //Set Interables to Saved State
        for (int i = 0; i < batteriesList.Count; i++)
        {
            if (currentSaveGame.savedBatteries.ContainsKey(batteriesList[i]))
            {
                batteriesList[i].isCollected = currentSaveGame.savedBatteries[batteriesList[i]];

                batteriesList[i].transform.parent.gameObject.SetActive(!batteriesList[i].isCollected);
            }
            else
            {
                Debug.LogWarning($"Battery {batteriesList[i].name} not found in saved data. Setting to default state.");
                batteriesList[i].isCollected = false; // default state if not found
            }
        }
        GM.batteriesDictionary = currentSaveGame.savedBatteries;
        GM.doorsDictionary = currentSaveGame.savedDoors;
        GM.buttonsDictionary = currentSaveGame.savedButtons;

        UpdateInteractablesList();
    }
}
