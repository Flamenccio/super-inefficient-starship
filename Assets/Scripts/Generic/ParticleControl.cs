using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : Effect
{
    [SerializeField] private float lifetime = 0.5f;
    protected override void Start()
    {
        animLength = lifetime;
        animator = null;
    }
}
