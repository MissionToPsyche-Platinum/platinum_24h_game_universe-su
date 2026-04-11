using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    private float volume = 1f;
    private bool colorblindMode = false;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public bool toggleColorblindMode()
    {
        colorblindMode = !colorblindMode;
        return colorblindMode;
    }
}
