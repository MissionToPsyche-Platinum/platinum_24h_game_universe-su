using TMPro;
using UnityEngine;

public class ColorblindButton : MonoBehaviour
{
    private Settings settingsInstance;
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        settingsInstance = Settings.instance;
    }

    public void toggleColorblindMode()
    {
        if (settingsInstance.toggleColorblindMode())
        {
            text.text = "ON";
        } else
        {
            text.text = "OFF";
        }

    }
}
