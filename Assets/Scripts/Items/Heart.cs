using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Item 
{
    protected override void TriggerEffect(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.heartCollect, transform.position);
        }
        base.TriggerEffect(collider);
    }
}
