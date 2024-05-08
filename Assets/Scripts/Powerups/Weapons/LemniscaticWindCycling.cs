using UnityEngine;
using Flamenccio.Attack.Player;
using Flamenccio.Effects;
using Flamenccio.Effects.Visual;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Powerup.Weapon
{
    public class LemniscaticWindCycling : WeaponSpecial
    {
        [SerializeField] private GameObject shockwaveEffect;
        private LemniscaticWindCyclingBullet attackInstance;
        private bool rechargeUsed = false;
        private const float DURATION = 0.10f;
        private const float SPEED = 50f;
        protected override void Startup()
        {
            base.Startup();
            Name = "Burst";
            Desc = "[TAP]: Rushes forward, dealing damage to any enemies in your path.\nIf at least 3 enemies are struck in one dash, grants 1 SPECIAL CHARGE.";
            Level = 1;
            Rarity = PowerupRarity.Rare;
        }
        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            PlayerMotion pm = PlayerMotion.Instance;

            if (pm.AimRestricted || pm.MovementRestricted) return;

            if (!playerAtt.UseCharge(cost)) return;

            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerSpecialBurst, transform.position);
            pm.RestrictAim(DURATION);
            pm.Blink(DURATION);
            rechargeUsed = false;
            Instantiate(shockwaveEffect, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
            attackInstance = Instantiate(mainAttack, PlayerMotion.Instance.transform, false).GetComponent<LemniscaticWindCyclingBullet>();
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Weak, pm.transform.position);
            float r = aimAngleDeg * Mathf.Deg2Rad;
            pm.Move(new Vector2(Mathf.Cos(r), Mathf.Sin(r)), SPEED, DURATION);
        }
        public override void Run()
        {
            if (!rechargeUsed && attackInstance != null && attackInstance.EnemiesHit >= 3)
            {
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerSpecialQue, transform.position);
                rechargeUsed = true;
                playerAtt.ReplenishCharge(1);
                EffectManager.Instance.SpawnEffect(EffectManager.Effects.SpecialReplenish, PlayerMotion.Instance.transform);
            }
        }
    }
}
