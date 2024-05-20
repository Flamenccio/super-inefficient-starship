using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.Core;
using Flamenccio.Effects.Audio;

namespace Flamenccio.LevelObject.Walls
{
    public class Wall : Destructables
    {
        [Tooltip("Game objects with these tags will destroy this object.")]
        [SerializeField] private List<string> dangerousTags = new List<string>();
        [SerializeField] private GameObject destroyEffect;
        [SerializeField] private GameObject spawnEffect;
        [SerializeField] private Sprite level0;
        [SerializeField] private Sprite level1;
        [SerializeField] private SpriteRenderer spriteRen;

        private const float MAX_LIFE = 90f;
        private float maxLifeSpan = 90.0f;
        private int level = 0; // level 0 is default
        private void Awake()
        {
            maxLifeSpan = MAX_LIFE;
            Instantiate(spawnEffect, transform);
        }
        private void Update()
        {
            if (maxLifeSpan <= 0) Die();
            maxLifeSpan -= Time.deltaTime;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (dangerousTags.Contains(collision.gameObject.tag))
            {
                currentHP -= collision.GetComponent<BulletControl>().ObjectDamage;
                if (currentHP <= 0)
                {
                    if (level == 0)
                    {
                        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.wallDestroy, transform.position);
                        Die();
                    }
                    Downgrade();
                }
            }
        }
        public void Die()
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        // upgrades wall to level 1 and resets life timer
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
