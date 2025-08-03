using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("References")]
    public SaveGame saveGame;
    public RewindData rewindData;

    GameManager GM;

    private void Awake()
    {
        GM = GameManager.GM;
    }


    public void ActivateCheckpoint()
    {
        ResetSaveFile();

        SaveNewSaveFile();
    }


    void ResetSaveFile()
    {
        saveGame.savedIsDeviceCollected = false;
        saveGame.savedRewindTimeBank = 0f;
        saveGame.savedObjectStatesInTime.Clear();
        saveGame.savedGameTime = 0f;

        saveGame.savedBatteries.Clear();
        saveGame.savedDoors.Clear();
        saveGame.savedButtons.Clear();
    }

    void SaveNewSaveFile()
    {
        saveGame.savedPlayerPosition = rewindData.transform.parent.position;
        saveGame.savedPlayerRotation = rewindData.transform.parent.rotation;

        saveGame.savedIsDeviceCollected = rewindData.isDeviceCollected;
        saveGame.savedRewindTimeBank = rewindData.rewindTimeBank;
        saveGame.savedObjectStatesInTime = rewindData.objectStatesInTime;
        saveGame.savedGameTime = GM.gameTime;

        GM.CreateInteractablesList();

        saveGame.savedBatteries = GM.batteriesDictionary;
        saveGame.savedDoors = GM.doorsDictionary;
        saveGame.savedButtons = GM.buttonsDictionary;

        GM.currentSaveGame = saveGame;
    }
}
