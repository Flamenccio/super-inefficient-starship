using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Powerup.Weapon
{
    public class Blaster : WeaponMain
    {
        [SerializeField] private GameObject chargeAttack;
        [SerializeField] private int ChargeCost = 1;
        protected override void Startup()
        {
            base.Startup();
            Name = "Blaster";
            Desc = "[TAP]: Fires a short-ranged bullet.\nDamage: low\nRange: low\nSpeed: Very fast\nCooldown: Very short";
            Rarity = PowerupRarity.Common;
            cost = 1;
        }
        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (cooldownTimer < cooldown) return; // don't do anything if on cooldown

            if (!consumeAmmo(Cost, PlayerAttributes.AmmoUsage.MainTap)) return; // don't do anything if there is not enough ammo

            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerShoot, transform.position);
            Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
            cooldownTimer = 0f;
        }
        public override void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!consumeAmmo(ChargeCost, PlayerAttributes.AmmoUsage.MainHoldExit)) return;

            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerSpecialBurst, transform.position);
            Instantiate(chargeAttack, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
        }
    }
}
