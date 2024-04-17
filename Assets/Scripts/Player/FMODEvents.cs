using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: SerializeField] public EventReference playerShoot { get; private set; }
    [field: SerializeField] public EventReference playerHurt { get; private set; }
    [field: SerializeField] public EventReference playerDash { get; private set; }
    [field: SerializeField] public EventReference crosshairsLockon { get; private set; }
    [field: SerializeField] public EventReference enemyHurt { get; private set; }
    [field: SerializeField] public EventReference enemyKill { get; private set; }
    [field: SerializeField] public EventReference starCollect {get; private set;}
    [field: SerializeField] public EventReference miniStarCollect { get; private set; }
    [field: SerializeField] public EventReference playerShootMini { get; private set; }
    [field: SerializeField] public EventReference playerSpecialBurst { get; private set; }
    [field: SerializeField] public EventReference wallDestroy { get; private set; }
    [field: SerializeField] public EventReference heartCollect { get; private set; }
    [field: SerializeField] public EventReference playerSpecialQue {  get; private set; }
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
