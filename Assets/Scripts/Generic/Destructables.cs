using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructables : MonoBehaviour
{
    [SerializeField] protected int maxHP = 0;
    protected int currentHP = 0;
    [SerializeField] protected int loot = 0;

    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    public int MaxHP { get => maxHP; set => maxHP = value; }
    public int CurrentHP { get => currentHP;}
    public int Loot { get => loot; set => loot = value; }
}
