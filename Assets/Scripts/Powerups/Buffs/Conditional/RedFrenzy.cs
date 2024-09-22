using Flamenccio.Core;
using Flamenccio.Effects;
using Flamenccio.HUD;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Powerup.Buff
{
    /// <summary>
    /// A buff with the following effects:
    /// <para>1. Removes cost for using Red Sword.</para>
    /// <para>2. Continuously drains ammo; rate increases with level.</para>
    /// <para>3. Buffs movement speed; buff increases with level.</para>
    /// </summary>
    public class RedFrenzy : ConditionalBuff
    {
        private float drainTimer = 0f;
        private float drainTime = 1f;
        private AmmoCostModifier localAmmoCostModifier;
        private const float MIN_AMMO_PERCENTAGE = 0.3f;
        private const int LOSS_AMMO_DEDUCTION = 10;
        private const float MOVE_SPEED_BUFF = 0.02f;

        // workarounds
        private Action<GameEventInfo> deactivateBuff;
        private Action<GameEventInfo> levelUpBuff;
        private Action<GameEventInfo> showLevelText;

        public RedFrenzy(PlayerAttributes p, Action<List<PlayerAttributes.Attribute>> a)
        {
            levelBuff = a;
            attributes = p;
            OnCreate();
        }

        protected override void OnCreate()
        {
            WeaponID = "Red Frenzy";
            static float SpeedBuff(int level) => level * MOVE_SPEED_BUFF;
            buffs.Add(new StatBuff(PlayerAttributes.Attribute.MoveSpeed, SpeedBuff));

            // set Actions to methods
            // this is a workaround to remove lambda expressions from Actions
            deactivateBuff = (_) => Deactivate();
            levelUpBuff = (_) => LevelUp();
            showLevelText = (x) => ShowLevelText(x.EventOrigin);

            GameEventManager.OnPlayerHit += deactivateBuff;
            GameEventManager.OnEnemyKill += levelUpBuff;
            GameEventManager.OnEnemyKill += showLevelText;
            attributes.AddAmmo(attributes.MaxAmmo); // TODO breaks when scene is reloaded: attributes is null
            localAmmoCostModifier = attributes.AddLocalAmmoCostModifier(PlayerAttributes.AmmoUsage.MainTap, false);
        }

        private float DrainSpeed(int level)
        {
            return Mathf.Clamp(2f / Mathf.Ceil(level / 8f), 0.4f, 2.0f);
        }

        public override void Run()
        {
            if (Level == 0) return;

            float ammoFill = attributes.Ammo / (float)attributes.MaxAmmo;

            if (ammoFill < MIN_AMMO_PERCENTAGE)
            {
                Level = 0;
                Deactivate();
                drainTimer = 0f;
                return;
            }

            if (drainTimer >= drainTime)
            {
                attributes.UseAmmo(1);
                drainTimer = 0f;
            }
            else
            {
                drainTimer += Time.deltaTime;
            }
        }

        protected override void OnLevelChange(int newLevel, int oldLevel)
        {
            if (oldLevel == 0) // this means that the level BEFORE the change is 0
            {
                localAmmoCostModifier.SetMultiplier(0);
            }
            if (newLevel > 0)
            {
                drainTime = DrainSpeed(Level);
            }
            else if (newLevel == 0)
            {
                localAmmoCostModifier.ResetMultiplier();
            }

            levelBuff?.Invoke(GetAffectedAttributes());
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameEventManager.OnPlayerHit -= deactivateBuff;
            GameEventManager.OnEnemyKill -= levelUpBuff;
            GameEventManager.OnEnemyKill -= showLevelText;
            attributes.RemoveLocalAmmoCostModifier(localAmmoCostModifier);
        }

        private void ShowLevelText(Vector2 pos)
        {
            if (Level > 0)
            {
                FloatingTextManager.Instance.DisplayText(Level.ToString(), pos, Color.red, 1.0f, 20.0f, FloatingTextControl.TextAnimation.ZoomOut, FloatingTextControl.TextAnimation.Fade, true);
            }
        }

        protected override void Deactivate()
        {
            float ammoFill = (float)attributes.Ammo / attributes.MaxAmmo;

            if (ammoFill >= MIN_AMMO_PERCENTAGE) attributes.UseAmmo(LOSS_AMMO_DEDUCTION);

            if (Level > 0)
            {
                // TODO localize please
                FloatingTextManager.Instance.DisplayText("FRENZY LOST", PlayerMotion.Instance.PlayerPosition, Color.red, 1.0f, 30.0f, FloatingTextControl.TextAnimation.ZoomOut, FloatingTextControl.TextAnimation.Fade, true);
            }

            base.Deactivate();
        }

        public override void LevelUp()
        {
            float ammoFill = (float)attributes.Ammo / attributes.MaxAmmo;

            if (ammoFill > MIN_AMMO_PERCENTAGE) base.LevelUp();
        }
    }
}