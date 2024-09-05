using UnityEngine;
using Flamenccio.Effects.Audio;
using FMODUnity;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Weapons that fires a simple bullet on TAP; fires a piercing bullet on HOLD.
    /// </summary>
    public class Blaster : WeaponMain
    {
        [SerializeField] private GameObject chargeAttack;
        [SerializeField] private int ChargeCost = 1;
        [SerializeField] private string tapSfx;
        [SerializeField] private string holdExitSfx;
        [SerializeField] private string chargedVfx;

        protected override void Startup()
        {
            base.Startup();
            Rarity = PowerupRarity.Common;
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            AudioManager.Instance.PlayOneShot(tapSfx, transform.position);
            Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
            cooldownTimer = 0f;
        }

        public override void HoldEnter(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            EffectManager.Instance.SpawnEffect(chargedVfx, transform);
        }

        public override void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!consumeAmmo(ChargeCost, PlayerAttributes.AmmoUsage.MainHoldExit)) return;

            AudioManager.Instance.PlayOneShot(holdExitSfx, transform.position);
            Instantiate(chargeAttack, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
        }
    }
}