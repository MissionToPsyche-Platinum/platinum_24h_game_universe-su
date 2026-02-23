using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FlightFactsPopup : MonoBehaviour
{
    [System.Serializable]
    private class FunFactEntry
    {
        public string category;
        public string fact;
    }

    [System.Serializable]
    private class FlightOverview
    {
        public string title;
        public string description;
    }

    [System.Serializable]
    private class FlightFactsData
    {
        public FlightOverview flightOverview;
        public FunFactEntry[] funFacts;
    }

    [SerializeField] private float timeBetweenFacts = 30f;
    [SerializeField] private float displayDuration = 8f;
    [SerializeField] private float fadeDuration = 1f;

    private FlightFactsData factsData;
    private GameObject popupPanel;
    private TextMeshProUGUI categoryText;
    private TextMeshProUGUI factText;
    private CanvasGroup canvasGroup;
    private int lastFactIndex = -1;

    private void Start()
    {
        LoadFacts();
        CreatePopupUI();
        StartCoroutine(ShowFactsLoop());
    }

    private void LoadFacts()
    {
        TextAsset json = Resources.Load<TextAsset>("FlightFacts");
        if (json == null)
        {
            string path = System.IO.Path.Combine(Application.dataPath, "Data", "FlightFacts.json");
            if (System.IO.File.Exists(path))
            {
                string jsonText = System.IO.File.ReadAllText(path);
                factsData = JsonUtility.FromJson<FlightFactsData>(jsonText);
            }
        }
        else
        {
            factsData = JsonUtility.FromJson<FlightFactsData>(json.text);
        }
    }

    private void CreatePopupUI()
    {
        // Create overlay canvas
        GameObject canvasObj = new GameObject("FactsPopupCanvas");
        canvasObj.transform.SetParent(transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Create panel with CanvasGroup for fading
        popupPanel = new GameObject("FactPanel");
        popupPanel.transform.SetParent(canvasObj.transform, false);
        canvasGroup = popupPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        RectTransform panelRect = popupPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.15f, 0.02f);
        panelRect.anchorMax = new Vector2(0.85f, 0.18f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = popupPanel.AddComponent<Image>();
        panelImage.color = new Color(0.05f, 0.05f, 0.15f, 0.85f);

        // Category text (top of panel)
        GameObject catObj = new GameObject("Category");
        catObj.transform.SetParent(popupPanel.transform, false);
        RectTransform catRect = catObj.AddComponent<RectTransform>();
        catRect.anchorMin = new Vector2(0, 0.55f);
        catRect.anchorMax = new Vector2(1, 1f);
        catRect.offsetMin = new Vector2(20, 0);
        catRect.offsetMax = new Vector2(-20, -5);

        categoryText = catObj.AddComponent<TextMeshProUGUI>();
        categoryText.fontSize = 18;
        categoryText.fontStyle = FontStyles.Bold | FontStyles.Italic;
        categoryText.color = new Color(0.7f, 0.8f, 1f, 1f);
        categoryText.alignment = TextAlignmentOptions.BottomLeft;

        // Fact text (bottom of panel)
        GameObject factObj = new GameObject("Fact");
        factObj.transform.SetParent(popupPanel.transform, false);
        RectTransform factRect = factObj.AddComponent<RectTransform>();
        factRect.anchorMin = new Vector2(0, 0);
        factRect.anchorMax = new Vector2(1, 0.55f);
        factRect.offsetMin = new Vector2(20, 5);
        factRect.offsetMax = new Vector2(-20, 0);

        factText = factObj.AddComponent<TextMeshProUGUI>();
        factText.fontSize = 16;
        factText.fontStyle = FontStyles.Italic;
        factText.color = Color.white;
        factText.alignment = TextAlignmentOptions.TopLeft;
    }

    private IEnumerator ShowFactsLoop()
    {
        // Wait a bit before showing the first fact
        yield return new WaitForSeconds(10f);

        while (true)
        {
            ShowRandomFact();

            // Fade in
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

            // Display
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

            // Wait before next fact
            yield return new WaitForSeconds(timeBetweenFacts);
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private void ShowRandomFact()
    {
        if (factsData == null || factsData.funFacts == null || factsData.funFacts.Length == 0)
            return;

        int index;
        do
        {
            index = Random.Range(0, factsData.funFacts.Length);
        } while (index == lastFactIndex && factsData.funFacts.Length > 1);

        lastFactIndex = index;
        FunFactEntry entry = factsData.funFacts[index];
        categoryText.text = "DID YOU KNOW? \u2014 " + entry.category;
        factText.text = "\"" + entry.fact + "\"";
    }
}
