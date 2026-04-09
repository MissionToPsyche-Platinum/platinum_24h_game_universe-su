using UnityEngine;
using TMPro;

/// <summary>
/// Updates a TextMeshProUGUI element with the current live score from ScoreManager.
/// Attach to the same GameObject as the text, or any object, and drag the text in.
/// </summary>
public class ScoreDisplay : MonoBehaviour {

    [Tooltip("The text element that displays the score.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Tooltip("Prefix shown before the number.")]
    [SerializeField] private string prefix = "Score: ";

    [Tooltip("How often (seconds) to refresh the displayed score. 0 = every frame.")]
    [SerializeField] private float refreshInterval = 0.1f;

    private float nextRefreshTime;

    private void Reset() {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if (Time.time < nextRefreshTime) return;
        nextRefreshTime = Time.time + refreshInterval;

        if (scoreText == null || ScoreManager.Instance == null) return;

        int score = Mathf.RoundToInt(ScoreManager.Instance.GetCurrentScore());
        scoreText.text = prefix + score;
    }
}
