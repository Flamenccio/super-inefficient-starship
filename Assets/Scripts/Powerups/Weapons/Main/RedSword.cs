using Flamenccio.Attack.Player;
using Flamenccio.Core;
using Flamenccio.Effects;
using UnityEngine;
using Flamenccio.Powerup.Buff;
using Flamenccio.Effects.Audio;
using Flamenccio.Effects.Visual;
using UnityEditor.PackageManager.UI;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// A main weapon that swings a sword on TAP. Launches a big, slow-moving projectile on HOLD.
    /// </summary>
    public class RedSword : WeaponMain
    {
        public int ChargeAttackDamage { get => chargeAttack.GetComponent<RedSwordChargeAttack>().PlayerDamage; }
        [SerializeField] private GameObject chargeAttack;
        [SerializeField] private string tapSfx;
        [SerializeField] private string chargedVfx;
        private const float SLASH_OFFSET = 1f;
        private bool flip = false;
        private int kills = 0;
        private BuffManager buffManager;

        protected override void Startup()
        {
            base.Startup();
            Level = 1;
            //Rarity = PowerupRarity.Uncommon;
            cooldownTimer = 0f;
            //playerAtt.AddAmmo(playerAtt.MaxAmmo);
        }

        private void OnEnable()
        {
            buffManager = GetComponentInParent<BuffManager>();
            buffManager.AddBuff(typeof(RedFrenzy));
            GameEventManager.OnEnemyKill += (_) => IncreaseKill();
            GameEventManager.OnPlayerHit += (_) => ResetKill();
        }

        private void OnDestroy()
        {
            if (!buffManager.RemoveBuff(typeof(RedFrenzy)))
            {
                Debug.LogWarning("failed to remove buff");
            }
            GameEventManager.OnEnemyKill -= (_) => IncreaseKill();
            GameEventManager.OnPlayerHit -= (_) => ResetKill();
        }

        private void IncreaseKill()
        {
            kills++;
        }

        private void ResetKill()
        {
            kills = 0;
        }

        public override void HoldEnter(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            EffectManager.Instance.SpawnEffect(chargedVfx, transform);
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            AudioManager.Instance.PlayOneShot(tapSfx, transform.position);
            flip = !flip;
            cooldownTimer = 0f;
            RedSwordAttack inst = Instantiate(mainAttack, PlayerMotion.Instance.transform).GetComponent<RedSwordAttack>();
            inst.transform.localPosition = new Vector2(SLASH_OFFSET, 0);
            inst.Flipped = flip;
        }

        public override void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!consumeAmmo(Cost1, PlayerAttributes.AmmoUsage.MainHoldExit))
            {
                return;
            }

            Instantiate(chargeAttack, PlayerMotion.Instance.PlayerPosition, Quaternion.Euler(0f, 0f, aimAngleDeg));
        }
    }
}