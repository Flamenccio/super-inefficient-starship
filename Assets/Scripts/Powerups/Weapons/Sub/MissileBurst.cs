using Flamenccio.Attack.Player;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    public class MissileBurst : WeaponSub
    {
        [SerializeField] private string lockonVfx;
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

        public override void Hold(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (loadedMissiles >= MISSILE_BURST_MAX_AMOUNT) return;

            loadingTimer -= Time.deltaTime;

            if (loadingTimer <= 0)
            {
                Debug.Log("Loaded a missile.");
                // TODO Add a small visual effect.
                loadedMissiles++;
                loadingTimer = LOADING_TIME_DEFAULT + ((MISSILE_BURST_MAX_AMOUNT - loadedMissiles) * LOADING_TIME_INCREASE);
            }
        }

        public override void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            loadingTimer = LOADING_TIME_DEFAULT;

            if (!AttackReady()) return;

            FireMissiles(aimAngleDeg, origin);
            loadedMissiles = MISSILE_BURST_DEFAULT_AMOUNT;
        }

        private void FireMissiles(float aimAngleDeg, Vector2 origin)
        {
            var targetList = PhysicsExtensions.ConeCastAll(origin, maxDistance, aimAngleDeg, MISSILE_SEARCH_DEGREES, targetLayers)
                    .Where(x => x.CompareTag(targetTag))
                    .ToList();
            float missileOffsetDegrees = 360f / loadedMissiles;

            for (int i = 0; i  < loadedMissiles; i++)
            {
                float angle = aimAngleDeg + (i * missileOffsetDegrees);
                var target = targetList.Count > 0 ? targetList[i % targetList.Count].transform : null;
                Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, angle)).GetComponent<PlayerMissile>().SetTarget(target);

                if (target != null) EffectManager.Instance.SpawnEffect(lockonVfx, target.position);
            }
        }
    }
}
