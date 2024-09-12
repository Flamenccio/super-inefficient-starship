using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// A main weapon that continuously fires a stream of inaccurate bullets on HOLD.
    /// </summary>
    public class Minigun : WeaponMain
    {
        public int BulletsPerRound { get => MAX_ROUNDS; }
        [SerializeField] private string holdSfx;

        private float attackDuration = 0f;
        private float freqTimer = 0f;
        private const int MAX_ROUNDS = 3; // rounds per ammo
        private int rounds = 0;

        private const float MIN_DEVIATION = 3.0f; // The minimum deviation of bullet spray.
        private const float MAX_DEVIATION = 20.0f; // The maximum deviation of bullet spray.
        private const float MIN_FREQUENCY = 3f / 60f;
        private const float MAX_FREQUENCY = 7f / 60f;
        private const float FIRE_TIMER_CAP = 3.0f; // The number of seconds on HOLD where fire frequency is slowest and most innaccurate.

        protected override void Startup()
        {
            base.Startup();
            Rarity = PowerupRarity.Common;
        }

        public override void Hold(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            attackDuration += Time.deltaTime;

            if (freqTimer <= 0f)
            {
                if (rounds <= 0)
                {
                    if (!consumeAmmo(Cost1, PlayerAttributes.AmmoUsage.MainHold))
                    {
                        return;
                    }

                    rounds = MAX_ROUNDS;
                }

                AudioManager.Instance.PlayOneShot(holdSfx, transform.position);
                Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, aimAngleDeg + GetDeviation(attackDuration)));
                freqTimer = GetFrequency(attackDuration);
                rounds--;
            }
            else
            {
                freqTimer -= Time.deltaTime;
            }
        }

        public override void HoldEnter(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            playerAtt.TemporaryAttributeChange(PlayerAttributes.Attribute.MoveSpeed, 0.33f);
        }

        public override void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            attackDuration = 0f;
            playerAtt.RestoreAttributeChange(PlayerAttributes.Attribute.MoveSpeed);
        }

        private float GetFrequency(float secondsPassed)
        {
            secondsPassed = Mathf.Clamp(secondsPassed, 0f, FIRE_TIMER_CAP);
            float slope = (MAX_FREQUENCY - MIN_FREQUENCY) / FIRE_TIMER_CAP;
            return slope * secondsPassed + MIN_FREQUENCY;
        }

        private float GetMaxDeviation(float secondsPassed)
        {
            secondsPassed = Mathf.Clamp(secondsPassed, 0f, FIRE_TIMER_CAP);
            float slope = (MAX_DEVIATION - MIN_DEVIATION) / FIRE_TIMER_CAP;
            return slope * secondsPassed + MIN_DEVIATION;
        }

        private float GetDeviation(float secondsPassed)
        {
            float maxDeviation = GetMaxDeviation(secondsPassed);
            return Random.Range(-maxDeviation, maxDeviation);
        }

    }
}