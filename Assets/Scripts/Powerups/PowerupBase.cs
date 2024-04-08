using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DEPCRATED CLASS! DON'T USE!
public class PowerupBase : MonoBehaviour
{
    protected string powerupName;
    protected string powerupDesc;
    protected int powerupLevel;
    protected PowerupRarity powerupRarity;

    public string Name { get => powerupName; }
    public string Desc { get => powerupDesc; }
    public int Level { get => powerupLevel; }
    public PowerupRarity Rarity { get => powerupRarity; }

    protected void Awake()
    {
        powerupLevel = 1;
        Startup();
    }
    protected virtual void Startup()
    {
        // do whatever
    }
    public void LevelChange(int levels)
    {
        if (powerupLevel < -levels) return; // if applying this change makes the level negative, don't
        powerupLevel += levels;
    }
    public virtual void Run()
    {
        // this function is meant to be run in Update()
    }
}
