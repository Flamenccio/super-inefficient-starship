using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Powerup.Weapon
{
    public class Blaster : WeaponMain
    {
        protected override void Startup()
        {
            AimAssisted = true;
            base.Startup();
            Name = "Blaster";
            Desc = "[TAP]: Fires a short-ranged bullet.\nDamage: low\nRange: low\nSpeed: Very fast\nCooldown: Very short";
            Rarity = PowerupRarity.Common;
            cost = 1;
            cooldown = 8f / 60f;
            mainAttack = Resources.Load<GameObject>("Prefabs/Bullets/Player/Player");
        }
        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (cooldownTimer < cooldown) return; // don't do anything if on cooldown

            if (!consumeAmmo(cost)) return; // don't do anything if there is not enough ammo

            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerShoot, transform.position);
            Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
            cooldownTimer = 0f;
        }
    }
}
