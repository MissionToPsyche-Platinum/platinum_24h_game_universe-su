using TMPro;
using UnityEngine;

public class ColorblindButton : MonoBehaviour
{
    private Settings settingsInstance;
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        settingsInstance = Settings.Instance;
        if (settingsInstance.colorblindMode)
        {
            text.text = "ON";
        } else
        {
            text.text = "OFF";
        }
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
