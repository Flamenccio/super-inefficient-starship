using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// A sub weapon that launches a grenade that explodes on impact or if it's lifetime reaches 0.
    /// </summary>
    public class Grenade : WeaponSub
    {
        protected override void Startup()
        {
            base.Startup();
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            Instantiate(mainAttack, transform.position, Quaternion.Euler(0f, 0f, aimAngleDeg));
            cooldownTimer = 0f;
        }
    }
}