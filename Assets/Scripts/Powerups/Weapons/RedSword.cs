using Flamenccio.Attack.Player;
using Flamenccio.Core;
using Flamenccio.Effects;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    public class RedSword : WeaponMain
    {
        [SerializeField] private GameObject chargeAttack;
        private const float SLASH_OFFSET = 1f;
        private bool flip = false;
        private int kills = 0;
        private PowerupManager powerupManager;

        protected override void Startup()
        {
            base.Startup();
            Name = "Red Sword";
            Desc = "[TAP]: Widely swings a red sword around you.\n[HOLD]: On release, unleashes a slow-moving vortex before you, repeatedly dealing damage in its path.\nTAP cost: 1\nHOLD cost: 3\nCooldown: fast";
            Level = 1;
            Rarity = PowerupRarity.Uncommon;
            cooldownTimer = 0f;
        }
        private void OnEnable()
        {
            powerupManager = GetComponentInParent<PowerupManager>();
            GameEventManager.OnEnemyKill += (_) => IncreaseKill();
            GameEventManager.OnPlayerHit += (_) => ResetKill();
        }
        private void OnDestroy()
        {
            GameEventManager.OnEnemyKill -= (_) => IncreaseKill();
            GameEventManager.OnPlayerHit -= (_) => ResetKill();
        }
        private void IncreaseKill()
        {
            kills++;
            BuffBase b = new RedFrenzy();
            powerupManager.AddBuff(b);
        }
        private void ResetKill()
        {
            kills = 0;
            powerupManager.RemoveBuff(new RedFrenzy());
        }
        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (cooldownTimer < cooldown) return;

            if (!flip)
            {
                if (!consumeAmmo(Cost)) // must check sequentially
                {
                    return;
                }
            }

            flip = !flip;
            cooldownTimer = 0f;
            RedSwordAttack inst = Instantiate(mainAttack, PlayerMotion.Instance.transform).GetComponent<RedSwordAttack>();
            inst.transform.localPosition = new Vector2(SLASH_OFFSET, 0);
            inst.Flipped = flip;
        }
        public override void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!consumeAmmo(Cost))
            {
                return;
            }

            Instantiate(chargeAttack, PlayerMotion.Instance.PlayerPosition, Quaternion.Euler(0f, 0f, aimAngleDeg));
        }
    }
}
