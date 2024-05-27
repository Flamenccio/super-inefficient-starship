using UnityEngine;
using Flamenccio.Effects.Audio;
using Flamenccio.Effects;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Quickly moves player in direction of movement input.
    /// </summary>
    public class Dash : WeaponDefense
    {
        [SerializeField] private GameObject afterImage;
        private float trailTimer = 0f;
        private const float DURATION = 5f / 60f;
        private const float SPEED = 50f;
        private const float TRAIL_FREQUENCY = 1f / 60f;

        protected override void Startup()
        {
            base.Startup();
            Desc = "[TAP]: Quickly move a set distance in direction you are moving in.";
            Name = "Dash";
            cost = 0;
            Rarity = PowerupRarity.Common;
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (cooldownTimer < cooldown) return; // don't do anything if on cooldown
            if (!PlayerMotion.Instance.RestrictMovement(DURATION)) return;
            float r = Mathf.Deg2Rad * moveAngleDeg;
            Vector2 v = new(Mathf.Cos(r), Mathf.Sin(r));
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerDash, transform.position);
            PlayerMotion.Instance.Move(v, SPEED, DURATION);
            cooldownTimer = 0f;
        }

        public override void Run()
        {
            base.Run();

            if (cooldownTimer <= DURATION)
            {
                if (trailTimer >= TRAIL_FREQUENCY)
                {
                    Instantiate(afterImage, transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                    trailTimer = 0f;
                }

                trailTimer += Time.deltaTime;
            }
            else
            {
                trailTimer = 0f;
            }
        }
    }
}