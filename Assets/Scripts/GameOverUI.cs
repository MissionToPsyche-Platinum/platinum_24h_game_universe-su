using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour {
    public static bool isVictory = false;

    [SerializeField] private TextMeshProUGUI titleText;

    private void Start() {
        if (isVictory) {
            titleText.text = "VICTORY!";
        } else {
            titleText.text = "MISSION FAILED";
        }
    }
}
