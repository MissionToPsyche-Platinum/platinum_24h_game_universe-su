using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    private float volume = 1f;
    public bool colorblindMode {get; private set;} = false;
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    public bool toggleColorblindMode()
    {
        colorblindMode = !colorblindMode;
        return colorblindMode;
    }
}
