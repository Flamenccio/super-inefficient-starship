using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.HUD;
using FMODUnity;
<<<<<<< HEAD
using UnityEditor.Build.Pipeline;
=======
>>>>>>> parent of dc4b1ce (Add system to select properties from GameObjects to populte local variables in LocalizedStrings)

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Base class for all main weapons.
    /// </summary>
    public class WeaponMain : WeaponBase
    {
        public int ChargedCost { get => chargedCost; }
        [SerializeField] protected int chargedCost = 0;

        protected override void Startup()
        {
            weaponType = WeaponType.Main;
            base.Startup();
        }

        protected override bool AttackReady()
        {
            if (cooldownTimer < Cooldown) return false;

            if (!consumeAmmo(Cost1, PlayerAttributes.AmmoUsage.MainTap))
            {
                // TODO Find a way to avoid hardcoding this text: "AMMO LOW."
                FloatingTextManager.Instance.DisplayText("AMMO LOW", transform.position, Color.yellow, 0.8f, 30f, FloatingTextControl.TextAnimation.ZoomOut, FloatingTextControl.TextAnimation.ZoomIn, true);
                return false;
            }

            return true;
        }

        protected override int GetDamage1()
        {
            return mainAttack.GetComponent<BulletControl>().PlayerDamage;
        }

        public float GetWeaponRange()
        {
            return mainAttack.GetComponent<BulletControl>().Range;
        }
    }
}