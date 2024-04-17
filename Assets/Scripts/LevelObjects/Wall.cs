using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Destructables
{
    [Tooltip("Game objects with these tags will destroy this object.")]
    [SerializeField] private List<string> dangerousTags = new List<string>();
    [SerializeField] private Spawner spawnControl;
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
        if (IsDangerousTag(collision.gameObject.tag))
        {
            currentHP -= collision.GetComponent<BulletControl>().Damage;
            if (currentHP <= 0)
            {
                if (level == 0)
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.wallDestroy, transform.position);
                    Die();
                }
                Downgrade();
            }
        }
    }
    // is the game object with the given tag able to destroy this one?
    private bool IsDangerousTag(string tag)
    {
        foreach (string tag2 in dangerousTags)
        {
            if (tag.Equals(tag2))
            {
                return true;
            }
        }
        return false;
    }
    public void Die()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        spawnControl.DecreaseWallCount();
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
