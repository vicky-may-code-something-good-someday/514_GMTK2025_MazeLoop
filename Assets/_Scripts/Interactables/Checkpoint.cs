using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("References")]

    GameManager GM;
    public SaveGame saveGame;
    public RewindData rewindData;

    private void Awake()
    {
        GM = GameManager.GM;
    }


    public void SaveGameAtCheckpoint()
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
        saveGame.savedPlayerPosition = transform.parent.position;
        saveGame.savedPlayerRotation = transform.parent.rotation;

        saveGame.savedIsDeviceCollected = rewindData.isDeviceCollected;
        saveGame.savedRewindTimeBank = rewindData.rewindTimeBank;
        saveGame.savedObjectStatesInTime.Clear();
        foreach (var state in rewindData.objectStatesInTime)
        {
            saveGame.savedObjectStatesInTime.Add(state);
        }
        saveGame.savedGameTime = GM.gameTime;

        GM.UpdateInteractablesList();

        saveGame.savedBatteries = GM.batteriesDictionary;
        saveGame.savedDoors = GM.doorsDictionary;
        saveGame.savedButtons = GM.buttonsDictionary;

        GM.currentSaveGame = saveGame;
    }
}
