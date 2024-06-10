using Flamenccio.Attack.Enemy;
using Flamenccio.Core;
using Flamenccio.Effects;
using Flamenccio.Effects.Audio;
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
        private readonly float conversionRatio = 0.7f;
        private readonly float timerReplenish = 3f;
        private const float PARRY_DURATION = 6f / 60f;
        private const float PARRY_RADIUS = 1.5f;
        private float parryTimer = 0f;
        private bool parry = false;
        private GameState gameState;
        private string[] attackTags;

        protected override void Startup()
        {
            base.Startup();
            Name = "Reload";
            Desc = $"Immediately consumes all stored star shards and converts {conversionRatio * 100f}% of them into ammo. Additionally restores {timerReplenish} seconds onto the life timer.\nMax charges: 1\nCooldown: {Cooldown} seconds";
            Rarity = PowerupRarity.Rare;
            gameState = FindObjectOfType<GameState>();
            parryTimer = 0f;
            attackTags = TagManager.GetTagCollection(new List<Tag> { Tag.NeutralBullet, Tag.EnemyBullet, }).ToArray();
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            // TODO make effects
            parryTimer = PARRY_DURATION;
            parry = true;
        }

        public override void Run()
        {
            base.Run();

            if (parry) Parry();
        }

        private List<GameObject> AttackScan(float radius)
        {
            return Physics2D.OverlapCircleAll(transform.position, radius)
                .Where(x => attackTags.Any(tag => x.CompareTag(tag)))
                .Select(x => x.gameObject)
                .ToList();
        }

        private void Parry()
        {
            parryTimer -= Time.deltaTime;

            if (AttackScan(PARRY_RADIUS).Count > 0)
            {
                PerfectReload(AttackScan(PARRY_RADIUS * 3f));
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
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetAudioEvent("PlayerSpecialReloadTap"), transform.position);
            int total = playerAtt.UseKillPoints();
            int final = Mathf.FloorToInt(total * conversionRatio);
            playerAtt.AddAmmo(final);
            gameState.ReplenishTimer(timerReplenish);
            cooldownTimer = Cooldown;
        }

        private void PerfectReload(List<GameObject> parriedObjects)
        {
            // TODO add a stun to all nearby enemies
            PlayerMotion.Instance.RestrictMovement(PARRY_DURATION);
            PlayerMotion.Instance.Move(transform.right, 20f, PARRY_DURATION);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetAudioEvent("PlayerSpecialReloadPerfectTap"), transform.position);
            int total = playerAtt.UseKillPoints();
            int final = Mathf.FloorToInt(total * conversionRatio);
            playerAtt.AddAmmo(final);
            gameState.ReplenishTimer(timerReplenish);
            cooldownTimer = Cooldown / 2f;
            parriedObjects
                .Where(x => x.TryGetComponent<EnemyBulletBase>(out var _))
                .ToList()
                .ForEach(x => Destroy(x));
        }
    }
}
