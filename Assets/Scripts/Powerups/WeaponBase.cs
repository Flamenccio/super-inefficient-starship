using Flamenccio.Core;
using System;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    public class WeaponBase : MonoBehaviour, IPowerup
    {
        public string Name { get; protected set; }
        public string Desc { get; protected set; }
        public int Level { get; protected set; }
        public PowerupRarity Rarity { get; protected set; }
        public WeaponType Type { get => weaponType; }
        public int Cost { get => cost; }
        public float Cooldown { get => cooldown; }
        public bool AimAssisted { get => aimAssisted; } // whether the weapon should aim towards the aim assist target on TAP
        public enum WeaponType
        {
            Main,
            Sub,
            Special,
            Defense,
            Support
        };
        [SerializeField] protected int cost = 1;
        [SerializeField] protected float cooldown;
        [SerializeField] protected float holdThreshold;
        [SerializeField] protected bool aimAssisted = false;
        [SerializeField] protected GameObject mainAttack;
        protected Func<int, bool> consumeAmmo;
        protected WeaponType weaponType;
        protected float cooldownTimer;
        protected PlayerAttributes playerAtt;
        
        private void Awake()
        {
            Level = 1;
            Startup();
        }
        protected virtual void Start()
        {
            playerAtt = GetComponentInParent<PlayerAttributes>();

            if (playerAtt == null)
            {
                Debug.LogError("ERROR: could not find a PlayerAttributes class in player.");
                return;
            }

            consumeAmmo = playerAtt.UseAmmo;
        }
        protected virtual void Startup()
        {
            cooldownTimer = 0f;
        }
        public virtual void Run()
        {

            if (cooldownTimer < cooldown)
            {
                cooldownTimer += Time.deltaTime;
            }
        }
        /// <summary>
        /// What weapon does when player taps button.
        /// </summary>
        /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
        /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
        public virtual void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
        /// <summary>
        /// The player effect that is called <b>once</b> when player enters hold attack.
        /// </summary>
        /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
        /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
        public virtual void HoldEnter(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
        /// <summary>
        /// The player effect that is run continuously while player is holding attack button.
        /// </summary>
        /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
        /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
        public virtual void Hold(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
        /// <summary>
        /// The player effect that is called <b>once</b> when player exits hold attack.
        /// </summary>
        public virtual void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
        public void LevelChange(int levels)
        {
            if (Level < -levels) return; // if applying this change makes the level negative, don't

            Level += levels;
        }
    }
}
