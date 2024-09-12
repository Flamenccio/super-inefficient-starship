using Flamenccio.Attack.Player;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    public class MissileBurst : WeaponSub
    {
        public int MissileAmountMin { get => MISSILE_BURST_DEFAULT_AMOUNT; }
        public int MissileAmountMax { get => MISSILE_BURST_MAX_AMOUNT; }

        [SerializeField] private string lockonVfx;
        [SerializeField] private string loadMissileVfx;
        private const int MISSILE_BURST_DEFAULT_AMOUNT = 4;
        private const int MISSILE_BURST_MAX_AMOUNT = 6;
        private const float MISSILE_SEARCH_DEGREES = 90f; // Width of search cone.
        private const float LOADING_TIME_DEFAULT = 0.3f;
        private const float LOADING_TIME_INCREASE = 0.10f;

        private float maxDistance = 10f;
        private int targetLayers;
        private string targetTag;
        private float loadingTimer = LOADING_TIME_DEFAULT;
        private int loadedMissiles = MISSILE_BURST_DEFAULT_AMOUNT;

        protected override void Startup()
        {
            maxDistance = mainAttack.GetComponent<PlayerBullet>().Range;
            targetLayers = LayerManager.GetLayerMask(Layer.Enemy);
            targetTag = TagManager.GetTag(Tag.Enemy);
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            Attack(aimAngleDeg, origin);
        }

        public override void Hold(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (cooldownTimer < Cooldown || loadedMissiles >= MISSILE_BURST_MAX_AMOUNT) return;

            loadingTimer -= Time.deltaTime;

            if (loadingTimer <= 0)
            {
                EffectManager.Instance.SpawnEffect(loadMissileVfx, transform);
                loadedMissiles++;
                loadingTimer = LOADING_TIME_DEFAULT + ((MISSILE_BURST_MAX_AMOUNT - loadedMissiles) * LOADING_TIME_INCREASE);
            }
        }

        public override void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            Attack(aimAngleDeg, origin);
        }

        private void Attack(float aimAngleDeg, Vector2 origin)
        {
            int load = loadedMissiles;
            loadedMissiles = MISSILE_BURST_DEFAULT_AMOUNT;
            loadingTimer = LOADING_TIME_DEFAULT;

            if (!AttackReady()) return;

            FireMissiles(aimAngleDeg, origin, load);
        }

        private void FireMissiles(float aimAngleDeg, Vector2 origin, int amount)
        {
            var targetList = PhysicsExtensions.ConeCastAll(origin, maxDistance, aimAngleDeg, MISSILE_SEARCH_DEGREES, targetLayers)
                    .Where(x => x.CompareTag(targetTag))
                    .ToList();
            float missileOffsetDegrees = 360f / amount;

            for (int i = 0; i  < amount; i++)
            {
                float angle = aimAngleDeg + (i * missileOffsetDegrees);
                var target = targetList.Count > 0 ? targetList[i % targetList.Count].transform : null;
                Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, angle)).GetComponent<PlayerMissile>().SetTarget(target);

                if (target != null) EffectManager.Instance.SpawnEffect(lockonVfx, target.position);
            }
        }
    }
}
