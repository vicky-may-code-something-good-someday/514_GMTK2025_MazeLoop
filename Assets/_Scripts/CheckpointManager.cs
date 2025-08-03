using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    //Singleton
    public static CheckpointManager Instance { get; private set; }

    public List<Checkpoint> checkpoints;


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


        checkpoints = new List<Checkpoint>(FindObjectsByType<Checkpoint>(FindObjectsSortMode.None));
    }


    public void SaveGameState()
    {
        // Implement logic to save the game state at the current checkpoint
        Debug.Log("Game state saved at checkpoint.");
    }

    public void LoadGameState()
    {
        //player controller reset
        //FreezeMovement();
        //UnfreezeMovement();


    }

}
