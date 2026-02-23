using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Adds a dark background panel for readability and a back button that returns to the main menu.
/// Attach to any scene's Canvas or empty GameObject.
/// </summary>
public class BackButtonUI : MonoBehaviour
{
    private void Start()
    {
        if (GameObject.Find("BackgroundCanvas") != null || GameObject.Find("BackButtonCanvas") != null)
        {
            return;
        }
        // Background canvas renders BEHIND the scene content
        GameObject bgCanvasObj = new GameObject("BackgroundCanvas");
        Canvas bgCanvas = bgCanvasObj.AddComponent<Canvas>();
        bgCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        bgCanvas.sortingOrder = -1;

        CanvasScaler bgScaler = bgCanvasObj.AddComponent<CanvasScaler>();
        bgScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        bgScaler.referenceResolution = new Vector2(1920, 1080);
        bgScaler.matchWidthOrHeight = 0.5f;

        GameObject panelObj = new GameObject("BackgroundPanel");
        panelObj.transform.SetParent(bgCanvasObj.transform, false);

        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.03f, 0.03f, 0.08f, 0.92f);
        panelImage.raycastTarget = false;   // IMPORTANT: don't block UI clicks

        // Button canvas renders ABOVE the scene content
        GameObject btnCanvasObj = new GameObject("BackButtonCanvas");
        Canvas btnCanvas = btnCanvasObj.AddComponent<Canvas>();
        btnCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        btnCanvas.sortingOrder = 100;

        CanvasScaler btnScaler = btnCanvasObj.AddComponent<CanvasScaler>();
        btnScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        btnScaler.referenceResolution = new Vector2(1920, 1080);
        btnScaler.matchWidthOrHeight = 0.5f;

        btnCanvasObj.AddComponent<GraphicRaycaster>();

        // Create button
        GameObject buttonObj = new GameObject("BackButton");
        buttonObj.transform.SetParent(btnCanvasObj.transform, false);

        RectTransform btnRect = buttonObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0, 1);
        btnRect.anchorMax = new Vector2(0, 1);
        btnRect.pivot = new Vector2(0, 1);
        btnRect.anchoredPosition = new Vector2(30, -20);
        btnRect.sizeDelta = new Vector2(180, 55);

        Image btnImage = buttonObj.AddComponent<Image>();
        btnImage.color = new Color(0.55f, 0.32f, 0.78f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => GameInput.Instance.SetMainMenuScene());

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "BACK";
        text.fontSize = 32;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        // Make existing scene text readable
        MakeSceneTextReadable();
    }

    private void MakeSceneTextReadable()
    {
        // Force all scene text to bright white
        foreach (TextMeshProUGUI tmp in FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
        {
            if (tmp.transform.parent != null && tmp.transform.parent.name == "BackButton") continue;

            tmp.color = Color.white;
        }

        // Also fix legacy UI Text components
        foreach (Text legacyText in FindObjectsByType<Text>(FindObjectsSortMode.None))
        {
            legacyText.color = Color.white;
        }
    }
}
