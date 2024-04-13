using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretStatic : EnemyShootBase, IEnemy
{
    public int Tier { get => tier; }

    [Tooltip("Where to fire bullets (in degrees).")]
    [SerializeField] private List<float> angles = new();

    protected override void Behavior()
    {
        // periodically shoot bullets in directions specified in list
        if (fireTimer >= fireRate)
        {
            Attack();
        }
        base.Behavior();
    }
    protected void Attack()
    {
        foreach (float angle in angles)
        {
            Fire(angle);
        }
    }

}
