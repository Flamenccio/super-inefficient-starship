using Flamenccio.Attack.Enemy;
using Flamenccio.Core;
using Flamenccio.Effects;
using Flamenccio.Effects.Audio;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// A special ability that instantly converts a portion of collected star shards (mini stars) into useable ammo.
    /// </summary>
    public class ReloadSpecialAbility : WeaponSpecial
    {
        [SerializeField] private string reloadNormalVfx;
        [SerializeField] private string reloadPerfectVfx;
        [SerializeField] private string reloadNormalSfx;
        [SerializeField] private string reloadPerfectSfx;

        private readonly float conversionRatio = 0.7f;
        private readonly float timerReplenish = 3f;

        private float parryTimer = 0f;
        private bool parry = false;
        private GameState gameState;
        private List<string> attackTags;

        private const float PARRY_DURATION = 6f / 60f;
        private const float PARRY_SCAN_RADIUS = 1.0f;
        private const float PARRY_DESTROY_RADIUS = PARRY_SCAN_RADIUS * 5f;
        private const float PARRY_PERFECT_REFUND = 0.50f;

        protected override void Startup()
        {

            /// TODO Find a solution to avoid hardcoding name and description. Applies to other buffs and weapons too.
            base.Startup();
            Name = "Reload";
            Desc = $"[TAP]: Immediately consumes all stored star shards and converts {conversionRatio * 100f}% of them into ammo. Additionally restores {timerReplenish} seconds onto the life timer.\nMax charges: {maxSpecialCharges}\nCooldown: {Cooldown} seconds.\nIf [TAP] is used right before a hit lands, performs a Perfect Reload: in addition of Reloading, restores {PARRY_PERFECT_REFUND * 100f}% of used star shards and cooldown is reduced to {Cooldown / 2f}.";
            Rarity = PowerupRarity.Rare;
            gameState = FindObjectOfType<GameState>();
            parryTimer = 0f;
            attackTags = TagManager.GetTagCollection(new List<Tag> { Tag.NeutralBullet, Tag.EnemyBullet, });
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            parryTimer = PARRY_DURATION;
            parry = true;
        }

        public override void Run()
        {
            base.Run();

            if (parry) Parry();
        }

        private List<GameObject> GameObjectScan(float radius, List<Tag> acceptedTags)
        {
            return GameObjectScan(radius, TagManager.GetTagCollection(acceptedTags));
        }

        private List<GameObject> GameObjectScan(float radius, List<string> acceptedTags)
        {
            return Physics2D.OverlapCircleAll(transform.position, radius)
                .Where(x => acceptedTags.Any(tag => x.CompareTag(tag)))
                .Select(x => x.gameObject)
                .ToList();
        }

        private void Parry()
        {
            parryTimer -= Time.deltaTime;

            if (GameObjectScan(PARRY_SCAN_RADIUS, attackTags).Count > 0)
            {
                PerfectReload();
                parry = false;
            }
            else if (parryTimer <= 0f)
            {
                Reload();
                parry = false;
            }
        }

        private void Reload()
        {
            PlayerMotion.Instance.Move(Vector2.zero, 0f, PARRY_DURATION * 2f);
            AudioManager.Instance.PlayOneShot(reloadNormalSfx, transform.position);
            EffectManager.Instance.SpawnEffect(reloadNormalVfx, transform.position);
            int total = playerAtt.UseKillPoints();
            int final = Mathf.FloorToInt(total * conversionRatio);
            playerAtt.AddAmmo(final);
            gameState.ReplenishTimer(timerReplenish);
            cooldownTimer = Cooldown;
        }

        private void PerfectReload()
        {
            PlayerMotion.Instance.Move(-transform.right, 20f, PARRY_DURATION);
            CameraEffects.Instance.SlowMo(0.5f);
            CameraEffects.Instance.Zoom(0.1f, 0.4f, -1f);
            AudioManager.Instance.PlayOneShot(reloadPerfectSfx, transform.position);
            EffectManager.Instance.SpawnEffect(reloadPerfectVfx, transform.position);
            int total = playerAtt.UseKillPoints();
            playerAtt.AddKillPoints(Mathf.FloorToInt(total * PARRY_PERFECT_REFUND));
            int final = Mathf.FloorToInt(total * conversionRatio);
            playerAtt.AddAmmo(final);
            gameState.ReplenishTimer(timerReplenish);
            cooldownTimer = Cooldown / 2f;

            GameObjectScan(PARRY_DESTROY_RADIUS, attackTags)
                .Where(x => x.TryGetComponent<EnemyBulletBase>(out var _))
                .ToList()
                .ForEach(x => Destroy(x));
        }
    }
}
