using UnityEngine;
using TMPro;

/// <summary>
/// Loads mission data from FunFacts.json and populates the scene UI.
/// All UI elements are set up in the scene; this script just fills in dynamic text.
/// Attach to a GameObject in MissionDetailsScene and assign TMP references in the Inspector.
/// </summary>
public class MissionInfoUI : MonoBehaviour
{
    [System.Serializable]
    private class FunFactEntry
    {
        public string category;
        public string fact;
    }

    [System.Serializable]
    private class MissionOverview
    {
        public string title;
        public string description;
    }

    [System.Serializable]
    private class FunFactsData
    {
        public MissionOverview missionOverview;
        public FunFactEntry[] funFacts;
    }

    [SerializeField] private TextMeshProUGUI overviewText;
    [SerializeField] private TextMeshProUGUI funFactCategoryText;
    [SerializeField] private TextMeshProUGUI funFactText;

    private FunFactsData factsData;

    private void Start()
    {
        LoadFunFacts();

        if (factsData != null && factsData.missionOverview != null && overviewText != null)
        {
            overviewText.text = factsData.missionOverview.description;
        }

        ShowRandomFact();
    }

    private void LoadFunFacts()
    {
        TextAsset json = Resources.Load<TextAsset>("FunFacts");
        if (json == null)
        {
            string path = System.IO.Path.Combine(Application.dataPath, "Data", "FunFacts.json");
            if (System.IO.File.Exists(path))
            {
                string jsonText = System.IO.File.ReadAllText(path);
                factsData = JsonUtility.FromJson<FunFactsData>(jsonText);
            }
        }
        else
        {
            factsData = JsonUtility.FromJson<FunFactsData>(json.text);
        }
    }

    private void ShowRandomFact()
    {
        if (factsData != null && factsData.funFacts != null && factsData.funFacts.Length > 0)
        {
            FunFactEntry entry = factsData.funFacts[Random.Range(0, factsData.funFacts.Length)];
            if (funFactCategoryText != null)
                funFactCategoryText.text = "DID YOU KNOW? \u2014 " + entry.category;
            if (funFactText != null)
                funFactText.text = "\"" + entry.fact + "\"";
        }
        else
        {
            if (funFactCategoryText != null)
                funFactCategoryText.text = "DID YOU KNOW?";
            if (funFactText != null)
                funFactText.text = "\"Asteroid Psyche may be the exposed core of an early planet, made mostly of iron and nickel.\"";
        }
    }
}
