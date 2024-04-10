using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Tooltip("Sound plays when player fires.")]
    [field: SerializeField] public EventReference playerShoot { get; private set; }
    [field: Tooltip("Sound plays when player is damaged.")]
    [field: SerializeField] public EventReference playerHurt { get; private set; }
    [field: Tooltip("Sound plays when player dashes.")]
    [field: SerializeField] public EventReference playerDash { get; private set; }
    [field: Tooltip("Sound plays when player crosshairs is on an enemy.")]
    [field: SerializeField] public EventReference crosshairsLockon { get; private set; }
    [field: Tooltip("Sound plays when enemy takes damage.")]
    [field: SerializeField] public EventReference enemyHurt { get; private set; }
    [field: Tooltip("Sound plays when enemy dies.")]
    [field: SerializeField] public EventReference enemyKill { get; private set; }
    [field: Tooltip("Sound plays when player collect star.")]
    [field: SerializeField] public EventReference starCollect {get; private set;}
    [field: Tooltip("Sound plays when player collects mini star.")]
    [field: SerializeField] public EventReference miniStarCollect { get; private set; }
    [field: SerializeField] public EventReference playerShootMini { get; private set; }
    public static FMODEvents instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of FMODEvents exists.");
        }
        instance = this;
    }
}
