using Flamenccio.Effects.Visual;

namespace Flamenccio.Attack.Player
{
    public class ShotgunBulletControl : PlayerBullet
    {
        protected override void Launch()
        {
            base.Launch();
            var i = EffectManager.Instance.SpawnTrail(TrailPool.Trails.SmokeTrail, transform.position).gameObject.GetComponent<SmokePuffControl>();
            i.Launch(rb.linearVelocity);
        }
    }
}
