using Flamenccio.Core;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// A special ability that instantly converts a portion of collected star shards (mini stars) into useable ammo.
    /// </summary>
    public class ReloadSpecialAbility : WeaponSpecial
    {
        private readonly float conversionRatio = 0.7f;
        private readonly float timerReplenish = 3f;
        private GameState gameState;

        protected override void Startup()
        {
            base.Startup();
            Name = "Reload";
            Desc = $"Immediately consumes all stored star shards and converts {conversionRatio * 100f}% of them into ammo. Additionally restores {timerReplenish} seconds onto the life timer.\nMax charges: 1\nCooldown: {Cooldown} seconds";
            Rarity = PowerupRarity.Rare;
            gameState = FindObjectOfType<GameState>();
        }

        public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
        {
            if (!AttackReady()) return;

            // TODO make effects
            int total = playerAtt.UseKillPoints();
            int final = Mathf.FloorToInt(total * conversionRatio);
            playerAtt.AddAmmo(final);
            gameState.ReplenishTimer(timerReplenish);
        }
    }
}
