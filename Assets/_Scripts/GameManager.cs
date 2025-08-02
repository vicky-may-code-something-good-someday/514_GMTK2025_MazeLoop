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
    float rewindedTime = 0f;

    // Update is called once per frame
    void Update()
    {
        text_gameTime.text = (Time.time - rewindedTime).ToString("F2");
    }

    public void RewindGameTime(float amount, float rewindSpeed = 1f)
    {
        rewindedTime += amount * rewindSpeed + amount;
        text_gameTime.text = (Time.time - rewindedTime).ToString("F2");
    }
}
