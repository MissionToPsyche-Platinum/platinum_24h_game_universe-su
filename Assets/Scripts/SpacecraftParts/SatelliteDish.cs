using UnityEngine;
using UnityEngine.SceneManagement;

public class SatelliteDish : MonoBehaviour {
    
    [SerializeField] private float facingThreshold;

    private GameInput gameInput;
    private Spacecraft spacecraft;

    public void Awake() {
        gameInput = GameInput.Instance;
        spacecraft = Spacecraft.GetInstance();
    }
    
    public void Start() {
        gameInput.OnRepairShipPerformedAction += GameInput_OnRepairShipPerformedAction;
        gameInput.OnRepairShipCanceledAction += GameInput_OnRepairShipCanceledAction;
    }

    private bool IsFacingEarth() {
        Vector2 directionToEarth = (Earth.Instance.transform.position - transform.position).normalized;
        float dot = Vector2.Dot(transform.up, directionToEarth);

        return dot > facingThreshold;
    }

    private void GameInput_OnRepairShipPerformedAction(object sender, System.EventArgs e) {
        Debug.Log($"Satellite facing earth: {IsFacingEarth()}");
    }
    
    private void GameInput_OnRepairShipCanceledAction(object sender, System.EventArgs e) {
        Debug.Log("Done repairing ship");
    }
    
    
    private void OnDestroy() {
        gameInput.OnRepairShipPerformedAction -= GameInput_OnRepairShipPerformedAction;
        gameInput.OnRepairShipCanceledAction -= GameInput_OnRepairShipCanceledAction;
    }
}
