using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class BombTimerManager : MonoBehaviour
{
    [Header("Bomb Settings")]
    [Tooltip("Duration of the bomb countdown in seconds")]
    [SerializeField]
    private float bombDuration = 30f;

    [Header("UI References")]
    [Tooltip("TextMeshPro UI element to display the bomb timer")]
    [SerializeField]
    private TMP_Text bombTimerText;

    [Header("Events")]
    [Tooltip("Event invoked when the bomb timer reaches zero")]
    [SerializeField]
    private UnityEvent onTimeExpired;

    // Time when the bomb was activated (in gameTime seconds)
    private float bombStartTime;

    // Flag indicating whether the bomb countdown is active
    private bool isActive = false;

    // Last whole second value used to trigger per-second effects
    private int lastWholeSecond = -1;

    private void Start()
    {
        // Hide timer at start
        if (bombTimerText != null)
            bombTimerText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Activates the bomb countdown using the current GameManager gameTime.
    /// </summary>
    public void ActivateBomb()
    {
        // Record start time
        bombStartTime = GameManager.GM.gameTime;
        isActive = true;
        lastWholeSecond = Mathf.CeilToInt(bombDuration);
        // Timer remains hidden until it drops below full duration
    }

    private void Update()
    {
        if (!isActive)
            return;

        // Calculate elapsed time since activation
        float elapsed = GameManager.GM.gameTime - bombStartTime;
        float remaining = bombDuration - elapsed;

        // Determine visibility: show only when remaining is below full duration but above zero
        if (remaining < bombDuration && remaining > 0f)
        {
            if (!bombTimerText.gameObject.activeSelf)
                bombTimerText.gameObject.SetActive(true);
        }
        else
        {
            if (bombTimerText.gameObject.activeSelf)
                bombTimerText.gameObject.SetActive(false);
        }

        // Clamp remaining time and handle expiration
        if (remaining <= 0f)
        {
            isActive = false;
            remaining = 0f;

            // Invoke expiration event
            onTimeExpired?.Invoke();
        }

        // Update UI when visible
        if (bombTimerText.gameObject.activeSelf)
            UpdateTimerUI(remaining);

        // If in last 5 seconds, trigger pulsate and color effect per second
        if (remaining <= 5f && remaining > 0f)
        {
            int currentWholeSecond = Mathf.CeilToInt(remaining);
            if (currentWholeSecond != lastWholeSecond)
            {
                lastWholeSecond = currentWholeSecond;
                TriggerWarningEffects();
            }
        }
    }

    /// <summary>
    /// Updates the bomb timer UI text with formatted seconds and two-digit milliseconds.
    /// </summary>
    /// <param name="time">Remaining time in seconds.</param>
    private void UpdateTimerUI(float time)
    {
        // Clamp to zero
        float clamped = Mathf.Max(time, 0f);

        int seconds = Mathf.FloorToInt(clamped);
        int milliseconds = Mathf.FloorToInt((clamped - seconds) * 100); // two digits

        bombTimerText.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);
    }

    /// <summary>
    /// Triggers visual warning effects using DOTween: pulsate and glow red.
    /// </summary>
    private void TriggerWarningEffects()
    {
        // Pulsate scale briefly
        bombTimerText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f)
            .SetEase(Ease.OutQuad);

        // Glow red then back to white
        bombTimerText.DOColor(Color.red, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
