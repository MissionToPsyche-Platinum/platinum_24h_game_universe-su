using UnityEngine;
using TMPro;

public class FlightFactsUI : MonoBehaviour
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

    [SerializeField] private TextMeshProUGUI overviewText;
    [SerializeField] private TextMeshProUGUI funFactCategoryText;
    [SerializeField] private TextMeshProUGUI funFactText;

    private FlightFactsData factsData;

    private void Start()
    {
        LoadFlightFacts();

        if (factsData != null && factsData.flightOverview != null && overviewText != null)
        {
            overviewText.text = factsData.flightOverview.description;
        }

        ShowRandomFact();
    }

    private void LoadFlightFacts()
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
                funFactText.text = "\"The Psyche spacecraft uses Hall-effect thrusters that produce a blue glow of ionized xenon gas.\"";
        }
    }
}
