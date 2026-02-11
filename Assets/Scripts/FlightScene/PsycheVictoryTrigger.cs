using UnityEngine;

/// <summary>
/// Triggers the victory screen when the spacecraft gets close enough to Psyche.
/// Automatically added to the Psyche GameObject by SolarSystemSetup.
/// </summary>
public class PsycheVictoryTrigger : MonoBehaviour
{
    [SerializeField] private float victoryDistance = 15f;

    private Spacecraft spacecraft;
    private bool triggered = false;

    private void Update()
    {
        if (triggered) return;

        if (spacecraft == null)
        {
            spacecraft = Spacecraft.GetInstance();
            if (spacecraft == null) return;
        }

        float dist = Vector2.Distance(transform.position, spacecraft.transform.position);

        if (dist <= victoryDistance)
        {
            triggered = true;
            Debug.Log("Victory!");
            GameInput.Instance.SetGameOverScene(true);
        }
    }
}
