using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : EnemyBulletNormal 
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject explosionHitbox;

    protected override void Startup()
    {
        base.Startup();
        ignoredTags.Add("Wall");
        ignoredTags.Add("Trigger");
    }
    protected override void Trigger(Collider2D collider)
    {
        Instantiate(explosion, transform.position, Quaternion.identity); // spawn an explosion effect
        Instantiate(explosionHitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>().EditProperties(0f, 2f, damage, Hitbox.AttackType.Neutral, knockbackMultiplier); // spawn the actual hitbox
        //gameObject.SetActive(false);
        CameraEffects.instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Extreme, transform.position);
        base.Trigger(collider);
    }
}
