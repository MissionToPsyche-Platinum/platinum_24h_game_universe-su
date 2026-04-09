using UnityEngine;
using TMPro;

/// <summary>
/// In-scene popup that shows the score breakdown on victory or death.
/// Place on a Canvas panel in the Flight scene and assign the TMP fields.
/// </summary>
public class ScoreBreakdownPopup : MonoBehaviour {

    public static ScoreBreakdownPopup Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI breakdownText;

    [SerializeField] private bool showOnOrbitEntry = true;

    private void Awake() {
        Instance = this;
        if (showOnOrbitEntry) OrbitAssist.OnEnteredOrbit += OrbitAssist_OnEnteredOrbit;
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    private void OrbitAssist_OnEnteredOrbit(object sender, System.EventArgs e) {
        ShowVictory();
    }

    private void OnDestroy() {
        OrbitAssist.OnEnteredOrbit -= OrbitAssist_OnEnteredOrbit;
        if (Instance == this) Instance = null;
    }

    public void ShowVictory() => Show(victory: true);
    public void ShowDeath()   => Show(victory: false);

    public void Show(bool victory) {
        if (ScoreManager.Instance == null) return;

        var b = ScoreManager.Instance.FinalizeRun(completed: victory, died: !victory);

        if (titleText != null) titleText.text = victory ? "VICTORY!" : "MISSION FAILED";

        if (breakdownText != null) {
            breakdownText.text =
                $"Base Score:        {Mathf.RoundToInt(b.baseScore)}\n" +
                $"Time Penalty:     -{Mathf.RoundToInt(b.timePenalty)}  ({b.elapsedSeconds:F1}s)\n" +
                $"Fuel Penalty:      -{Mathf.RoundToInt(b.fuelPenalty)}  ({b.fuelUsedPercent:F0}%)\n" +
                $"Damage Penalty: -{Mathf.RoundToInt(b.damagePenalty)}  ({b.damageTakenPercent:F0}%)\n" +
                $"Completion Bonus: +{Mathf.RoundToInt(b.completionBonus)}\n" +
                $"\n" +
                $"FINAL SCORE: {Mathf.RoundToInt(b.finalScore)}";
        }

        if (panelRoot != null) panelRoot.SetActive(true);
    }
}
