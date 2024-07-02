using Flamenccio.Attack.Player;
using Flamenccio.Utility;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    public class MissileBurst : WeaponSub
    {
        private const int MISSILE_BURST_AMOUNT = 8;
        private const float MISSILE_OFFSET_DEGREES = 360f / MISSILE_BURST_AMOUNT;
        private const float MAX_DISTANCE = 40f;
        private int targetLayers;
        private string targetTag;

        protected override void Startup()
        {
            targetLayers = LayerManager.GetLayerMask(Layer.Enemy);
            targetTag = TagManager.GetTag(Tag.Enemy);
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            for (int i = 0; i  < MISSILE_BURST_AMOUNT; i++)
            {
                float angle = aimAngleDeg + (i * MISSILE_OFFSET_DEGREES);
                var targetList = PhysicsExtensions.ConeCastAll(origin, MAX_DISTANCE, angle, MISSILE_OFFSET_DEGREES, targetLayers)
                    .Where(x => x.CompareTag(targetTag))
                    .ToList();
                var target = targetList.Count > 0 ? targetList[0].transform : null;
                Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, angle)).GetComponent<PlayerMissile>().SetTarget(target);
            }
        }
    }
}
