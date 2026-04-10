using UnityEngine;

/// <summary>
/// Tunable weights and base values for the run-scoring formula.
/// Create one via: Assets > Create > Scoring > Score Config
/// </summary>
[CreateAssetMenu(fileName = "ScoreConfig", menuName = "Scoring/Score Config")]
public class ScoreConfig : ScriptableObject {

    [Header("Base")]
    [Tooltip("Starting score before penalties are subtracted.")]
    public float baseScore = 10000f;

    [Tooltip("Minimum score the player can end with (clamp floor).")]
    public float minScore = 0f;

    [Header("Penalty Weights")]
    [Tooltip("Points lost per second of run time.")]
    public float timeWeightPerSecond = 10f;

    [Tooltip("Points lost per 1% of fuel consumed (0-100 scale).")]
    public float fuelWeightPerPercent = 30f;

    [Tooltip("Points lost per 1% of max HP lost (0-100 scale).")]
    public float damageWeightPerPercent = 50f;

    [Header("Bonuses")]
    [Tooltip("Flat bonus added on successful level completion.")]
    public float completionBonus = 0f;

    [Tooltip("If true, dying forces the score to minScore regardless of penalties.")]
    public bool zeroScoreOnDeath = false;
}
