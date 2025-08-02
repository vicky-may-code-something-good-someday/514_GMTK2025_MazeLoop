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

    [SerializeField] TMP_Text text_gameTime;
    //float rewindedTime = 0f;
    float gameTime = 0f;
    bool pauseGameTime = false;

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
}
