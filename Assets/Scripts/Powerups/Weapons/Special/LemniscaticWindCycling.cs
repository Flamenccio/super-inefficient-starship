using UnityEngine;
using Flamenccio.Attack.Player;
using Flamenccio.Effects;
using Flamenccio.Effects.Visual;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// A special weapon dash attack that replenishes a special charge if at least 3 enemies are hit in a dash.
    /// </summary>
    public class LemniscaticWindCycling : WeaponSpecial
    {
        public int HitStreakCondition { get => HIT_STREAK_CONDITION; }
        [SerializeField] private GameObject shockwaveEffect;
        [SerializeField] private float DURATION = 0.10f;
        [SerializeField] private float SPEED = 50f;
        [SerializeField] private int HIT_STREAK_CONDITION = 3; // number of enemies required to be hit to replenish a special charge.
        [SerializeField] private string tapSfx;
        [SerializeField] private string specialReplenishSfx;
        [SerializeField] private string specialReplenishVfx;

        private LemniscaticWindCyclingBullet attackInstance;
        private bool rechargeUsed = false;

        protected override void Startup()
        {
            base.Startup();
            Level = 1;
            Rarity = PowerupRarity.Rare;
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            PlayerMotion pm = PlayerMotion.Instance;

            if (pm.AimRestricted || pm.MovementRestricted) return;

            if (!AttackReady()) return;

            AudioManager.Instance.PlayOneShot(tapSfx, transform.position);
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Weak, pm.transform.position);
            Instantiate(shockwaveEffect, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));

            pm.RestrictAim(DURATION);
            pm.Blink(DURATION);
            rechargeUsed = false;
            attackInstance = Instantiate(mainAttack, PlayerMotion.Instance.transform, false).GetComponent<LemniscaticWindCyclingBullet>();
            float r = aimAngleDeg * Mathf.Deg2Rad;
            pm.Move(new Vector2(Mathf.Cos(r), Mathf.Sin(r)), SPEED, DURATION);
        }

        public override void Run()
        {
            if (!rechargeUsed && attackInstance != null && attackInstance.EnemiesHit >= 3)
            {
                AudioManager.Instance.PlayOneShot(specialReplenishSfx, transform.position);
                rechargeUsed = true;
                playerAtt.ReplenishCharge(1);
                EffectManager.Instance.SpawnEffect(specialReplenishVfx, PlayerMotion.Instance.transform);
            }
        }
    }
}