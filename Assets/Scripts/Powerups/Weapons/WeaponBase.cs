using Flamenccio.Effects.Audio;
using Flamenccio.Localization;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Base class for all player weapons.
    /// </summary>
    public class WeaponBase : MonoBehaviour, IPowerup, IDescribable
    {
        public string Name { get => weaponId; }
        public int Level { get; protected set; }
        public PowerupRarity Rarity { get; protected set; }
        public WeaponType Type { get => weaponType; }
        public int Cost { get => cost; }
        public float Cooldown { get => cooldown; }
        public bool AimAssisted { get => aimAssisted; } // whether the weapon should aim towards the aim assist target on TAP
        [SerializeField] private string weaponId;

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
        protected Func<int, PlayerAttributes.AmmoUsage, bool> consumeAmmo;
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

        /// <summary>
        /// Runs in Awake().
        /// </summary>
        protected virtual void Startup()
        {
            cooldownTimer = 0f;
        }

        /// <summary>
        /// Runs in Update().
        /// </summary>
        public virtual void Run()
        {
            if (cooldownTimer < Cooldown)
            {
                cooldownTimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// What weapon does when player taps button.
        /// </summary>
        /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
        /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
        public virtual void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        { }

        /// <summary>
        /// The player effect that is called <b>once</b> when player enters hold attack.
        /// </summary>
        /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
        /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
        public virtual void HoldEnter(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        { }

        /// <summary>
        /// The player effect that is run continuously while player is holding attack button.
        /// </summary>
        /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
        /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
        public virtual void Hold(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        { }

        /// <summary>
        /// The player effect that is called <b>once</b> when player exits hold attack.
        /// </summary>
        public virtual void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        { }

        /// <summary>
        /// Is this weapon ready to fire?
        /// </summary>
        protected virtual bool AttackReady()
        {
            return cooldownTimer >= Cooldown;
        }

        public virtual LocalizedString CompleteDescription(LocalizedString description)
        {
            // TODO make some way for dev to assign variable names to correct values
            return description; // Placeholder
        }
    }
}