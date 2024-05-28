using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Weapons that fires a simple bullet on TAP; fires a piercing bullet on HOLD.
    /// </summary>
    public class Blaster : WeaponMain
    {
        [SerializeField] private GameObject chargeAttack;
        [SerializeField] private int ChargeCost = 1;

        protected override void Startup()
        {
            base.Startup();
            Name = "Blaster";
            Desc = "[TAP]: Fires a short-ranged bullet.\n[HOLD]: Fires a piercing bullet.\nDamage: low\nRange: low\nSpeed: Very fast\nCooldown: Very short";
            Rarity = PowerupRarity.Common;
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

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