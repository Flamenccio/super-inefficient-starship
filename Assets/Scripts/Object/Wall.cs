using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.Core;
using Flamenccio.Effects.Audio;
using System;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Objects
{
    /// <summary>
    /// Controls the behavior of wall objects.
    /// </summary>
    public class Wall : Destructables, IObject
    {
        public Action OnDeath;
        public string GetObjectName { get => objectName; }
        [SerializeField] private string destroyVfx;
        [SerializeField] private string spawnVfx;
        [SerializeField] private string destroySfx;
        [SerializeField] private string objectName;
        [SerializeField] private Sprite level0;
        [SerializeField] private Sprite level1;
        [SerializeField] private SpriteRenderer spriteRen;

        private const float MAX_LIFE = 90f;
        private float maxLifeSpan = 90.0f;
        private int level = 0; // level 0 is default

        private void Awake()
        {
            maxLifeSpan = MAX_LIFE;
            EffectManager.Instance.SpawnEffect(spawnVfx, transform);
        }

        private void Update()
        {
            if (maxLifeSpan <= 0) Die();
            maxLifeSpan -= Time.deltaTime;
        }

        public void OnBulletHit(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out BulletControl bulletControl)) return;
            
            currentHP -= bulletControl.ObjectDamage;
            
            if (currentHP <= 0)
            {
                if (level == 0)
                {
                    AudioManager.Instance.PlayOneShot(destroySfx, transform.position);
                    Die();
                }
                Downgrade();
            }
        }

        public void Die()
        {
            OnDeath?.Invoke();
            EffectManager.Instance.SpawnEffect(destroyVfx, transform.position);
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Upgrades this wall and resets its lifetime.
        /// </summary>
        public void Upgrade()
        {
            if (level == 1) return;

            maxLifeSpan = MAX_LIFE;
            level = 1;
            currentHP = 1;
            spriteRen.sprite = level1;
        }

        private void Downgrade()
        {
            level = 0;
            currentHP = 1;
            spriteRen.sprite = level0;
        }
    }
}