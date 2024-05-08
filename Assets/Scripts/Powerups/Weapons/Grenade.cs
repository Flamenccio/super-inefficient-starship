using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    public class Grenade : WeaponSub
    {
        protected override void Startup()
        {
            base.Startup();
            Name = "Grenade";
            Desc = "[TAP]: throw a bomb that explodes on contact.\nDamage: high\nCooldown: 3 seconds\nRange: average\nCost: 5";
        }
        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (cooldownTimer < cooldown) return;

            if (!consumeAmmo(cost)) return;

            Instantiate(mainAttack, transform.position, Quaternion.Euler(0f, 0f, aimAngleDeg));
            cooldownTimer = 0f;
        }
    }
}
