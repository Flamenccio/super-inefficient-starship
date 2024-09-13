using Flamenccio.Attack;
using Flamenccio.Effects;
using Flamenccio.Effects.Audio;
using Flamenccio.Effects.Visual;
using UnityEngine;
using static Flamenccio.Attack.BulletControl;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Fires a spray of bullets and launches player backward.
    /// </summary>
    public class ShotgunWeapon : WeaponMain
    {
        [SerializeField] private GameObject playerTackleHitbox;
        [SerializeField] private string muzzleFlashVfx;
        [SerializeField] private string tapSfx;

        private const float DEVIATION_MAX_DEGREES = 50.0f;
        private const int BLAST_AMOUNT = 6; // bullets fired per shot
        private const float KNOCKBACK_DURATION = 0.24f;
        private const float TACKLE_DURATIION = 0.4f;
        private const float TACKLE_RADIUS = 1.6f;

        protected override void Startup()
        {
            base.Startup();
            //Rarity = PowerupRarity.Uncommon;
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            float deviation = 0f;
            cooldownTimer = 0f;
            var rad = Mathf.Deg2Rad * (aimAngleDeg + 180f);
            Vector2 opposite = new(Mathf.Cos(rad), Mathf.Sin(rad));

            PlayerMotion.Instance.RestrictMovement(KNOCKBACK_DURATION);
            PlayerMotion.Instance.Shove(opposite, (float)KnockbackPower.Extreme);
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Strong, origin);
            AudioManager.Instance.PlayOneShot(tapSfx, transform.position);
            EffectManager.Instance.SpawnEffect(muzzleFlashVfx, transform);

            var hitbox = Instantiate(playerTackleHitbox, PlayerMotion.Instance.PlayerTransform).GetComponent<Hitbox>();
            hitbox.EditProperties(TACKLE_DURATIION, TACKLE_RADIUS, 3, Hitbox.HitboxAffiliation.Player);

            for (int i = 0; i < BLAST_AMOUNT; i++)
            {
                Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, aimAngleDeg + deviation));
                deviation = Random.Range(-DEVIATION_MAX_DEGREES, DEVIATION_MAX_DEGREES);
            }
        }
    }
}
