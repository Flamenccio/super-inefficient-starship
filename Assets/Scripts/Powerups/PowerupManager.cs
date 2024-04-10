using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPowerup
{
    string Name { get; }
    string Desc { get; }
    int Level { get; }
    PowerupRarity Rarity { get; }
    void LevelChange(int l);
    void Run();
}
public enum PowerupRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
};

public class PowerupManager : MonoBehaviour
{
    private WeaponMain mainAttack;
    private WeaponSub subAttack;

    private Action powerupUpdate;

    private List<BuffBase> buffs = new List<BuffBase>();
    public List<BuffBase> Buffs { get => buffs; }

    [SerializeField] [Tooltip("Path to default weapon.")] private UnityEditor.MonoScript defaultMain;
    [SerializeField] [Tooltip("Path to default sub weapon.")] private UnityEditor.MonoScript defaultSub;
    private PlayerAttributes playerAttributes;

    private void Awake()
    {
        // set default attacks
        mainAttack = gameObject.AddComponent(defaultMain.GetClass()).GetComponent<WeaponMain>();
        subAttack = gameObject.AddComponent(defaultSub.GetClass()).GetComponent<WeaponSub>();

        playerAttributes = gameObject.GetComponent<PlayerAttributes>();

        powerupUpdate += mainAttack.Run;
        powerupUpdate += subAttack.Run;
    }
    public WeaponMain AddMain(WeaponMain main) // replaces main weapon with given one. Returns previous main weapon.
    {
        WeaponMain temp = mainAttack;
        powerupUpdate -= mainAttack.Run;
        Destroy(mainAttack); // remove the current main attack from player
        mainAttack = gameObject.AddComponent(main.GetType()).GetComponent<WeaponMain>(); // and replace it with the new one

        powerupUpdate += mainAttack.Run;

        return temp;
    }
    public WeaponSub AddSub(WeaponSub sub) // same thing as above
    {
        WeaponSub temp = subAttack;
        powerupUpdate -= subAttack.Run;
        Destroy(subAttack);
        subAttack = gameObject.AddComponent(sub.GetType()).GetComponent<WeaponSub>();

        powerupUpdate += subAttack.Run;
        return temp;
    }
    public void MainAttackTap(float angle, Vector2 origin, Func<int, bool> deductAmmo)
    {
        mainAttack.Execute(angle, origin, deductAmmo);
    }
    public void MainAttackHold(float angle, Vector2 origin, Func<int, bool> deductAmmo, float holdTime)
    {
        mainAttack.HoldExecute(angle, origin, deductAmmo, holdTime);
    }
    public void MainAttackEffectHoldEnter(Rigidbody2D rigidbody)
    {
        mainAttack.PlayerEffectHoldEnter(rigidbody);
    }
    public void MainAttackEffectHoldExit(Rigidbody2D rigidbody)
    {
        mainAttack.PlayerEffectHoldExit(rigidbody);
    }
    public void MainAttackEffectHoldContinuous(Rigidbody2D rigidbody)
    {
        mainAttack.PlayerEffectHold(rigidbody);
    }
    public void SubAttackTap(float angle, Vector2 origin, Func<int, bool> deductAmmo)
    {
        subAttack.Execute(angle, origin, deductAmmo);
    }
    public void SubAttackEffectTap(Rigidbody2D rigidbody)
    {
        subAttack.PlayerEffectTap(rigidbody);
    }
    private void Update()
    {
        powerupUpdate();
    }
    public void AddBuff(BuffBase b)
    {
        int x = FindBuff(b);
        if (x < 0)
        {
            //Debug.Log("added");
            buffs.Add(b); // if there is no existing duplciate, add it
        }
        else
        {
            //Debug.Log("Upgraded");
            buffs[x].LevelChange(1); // if there is an existing duplicate level up
        }
        //playerAttributes.CompileBonus(b);
        foreach(PlayerAttributes.Attribute a in b.GetAffectedAttributes())
        {
            playerAttributes.RecompileBonus(a, buffs);
        }
    }
    public bool RemoveBuff(BuffBase b)
    {
        int x = FindBuff(b);
        if (x < 0)
        {
            return false;
        }
        buffs.RemoveAt(x);
        buffs.Sort((BuffBase a, BuffBase b) => { return (a.Level < b.Level ? -1 : 1); }); // basically resort the list based on level: higher level buffs will be placed at the top.
        foreach (PlayerAttributes.Attribute a in b.GetAffectedAttributes())
        {
            playerAttributes.RecompileBonus(a, buffs);
        }
        return true;
    }
    /// <summary>
    /// Finds a buff from <b>buffs</b> and returns its index in the list. -1 if it doesn't exist.
    /// </summary>
    private int FindBuff(BuffBase b)
    {
        int i = 0;
        foreach (BuffBase bb in buffs)
        {
            if (bb.Name.Equals(b.Name))
            {
                return i;
            }
            i++;
        }
        return -1; // if there is no buff that exists
    }
}
