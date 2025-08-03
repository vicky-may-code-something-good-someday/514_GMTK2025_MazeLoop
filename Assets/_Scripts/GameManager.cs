using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
    }

    [Header("Interactables")]
    [HideInInspector] public List<Battery> batteriesList = new List<Battery>();
    [HideInInspector] public List<Door> doorsList = new List<Door>();
    [HideInInspector] public List<Button> buttonsList = new List<Button>();

    [HideInInspector] public Dictionary<Battery, bool> batteriesDictionary = new Dictionary<Battery, bool>();
    [HideInInspector] public Dictionary<Door, bool> doorsDictionary = new Dictionary<Door, bool>();
    [HideInInspector] public Dictionary<Button, bool> buttonsDictionary = new Dictionary<Button, bool>();

    public SaveGame currentSaveGame;

    public RewindData rewindData;


    [SerializeField] TMP_Text text_gameTime;
    //float rewindedTime = 0f;
    public float gameTime = 0f;
    bool pauseGameTime = false;


    void Start()
    {
        CreateInteractablesList();
    }


    void Update()
    {
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

    public void CreateInteractablesList()
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
        if (currentSaveGame == null) return;
        //restart the game?

        rewindData.transform.parent.position = currentSaveGame.savedPlayerPosition;
        rewindData.transform.parent.rotation = currentSaveGame.savedPlayerRotation;

        rewindData.isDeviceCollected = currentSaveGame.savedIsDeviceCollected;
        rewindData.rewindTimeBank = currentSaveGame.savedRewindTimeBank;
        rewindData.objectStatesInTime = currentSaveGame.savedObjectStatesInTime;

        gameTime = currentSaveGame.savedGameTime;



        for (int i = 0; i < batteriesList.Count; i++)
        {
            if (currentSaveGame.savedBatteries.ContainsKey(batteriesList[i]))
            {
                batteriesList[i].isCollected = currentSaveGame.savedBatteries[batteriesList[i]];
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

        CreateInteractablesList();
    }
}
