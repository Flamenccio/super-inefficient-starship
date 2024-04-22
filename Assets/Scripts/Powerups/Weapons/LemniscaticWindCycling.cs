using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.Effects;
using Flamenccio.Effects.Visual;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Powerup.Weapon
{
    public class LemniscaticWindCycling : WeaponSpecial
    {
        private const int MAX_CHARGES = 2;
        private GameObject shockwaveEffect;
        private GameObject attack;
        private LemniscaticWindCyclingBullet attackInstance;
        private bool rechargeUsed = false;
        private const float DURATION = 0.10f;
        private const float SPEED = 50f;
        protected override void Startup()
        {
            base.Startup();
            AimAssisted = true;
            Name = "Lemniscatic Wind Cycling";
            Desc = "[TAP]: Rushes forward, dealing damage to any enemies in your path.\nIf at least 3 enemies are struck in one dash, grants 1 SPECIAL CHARGE.";
            Level = 1;
            Rarity = PowerupRarity.Rare;
            cooldown = 3.0f;
            shockwaveEffect = Resources.Load<GameObject>("Prefabs/Effects/LemniscaticWindCyclingShockwave");
            attack = Resources.Load<GameObject>("Prefabs/Bullets/Player/LemniscaticWindCyclingAttack");
        }
        protected override void Start()
        {
            playerAtt.SetCharges(MAX_CHARGES, cooldown);
        }
        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            PlayerMotion pm = PlayerMotion.Instance;

            if (pm.AimRestricted || pm.MovementRestricted) return;

            if (!playerAtt.UseCharge(1)) return;

            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerSpecialBurst, transform.position);
            pm.RestrictAim(DURATION);
            pm.Blink(DURATION);
            rechargeUsed = false;
            Instantiate(shockwaveEffect, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
            attackInstance = Instantiate(attack, PlayerMotion.Instance.transform, false).GetComponent<LemniscaticWindCyclingBullet>();
            CameraEffects.instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Weak, pm.transform.position);
            float r = aimAngleDeg * Mathf.Deg2Rad;
            pm.Move(new Vector2(Mathf.Cos(r), Mathf.Sin(r)), SPEED, DURATION);
        }
        public override void Run()
        {
            if (!rechargeUsed && attackInstance != null && attackInstance.EnemiesHit >= 3)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerSpecialQue, transform.position);
                rechargeUsed = true;
                playerAtt.ReplenishCharge(1);
            }
        }
    }
}
