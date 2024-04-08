using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownControl : Effect
{
    [SerializeField] private Transform player;
    private const float baseDuration = 8f / 12f;
    protected override void Start()
    {
        base.Start();
    }
    public void Display(float duration)
    {
        float speedMultiplier = baseDuration / duration;
        timer = 0.0f;
        animator.SetFloat("speed", speedMultiplier);
        animator.SetBool("active", true);
        animLength = duration;
        gameObject.SetActive(true);
        animator.Play("cooldown", 0, 0f);
    }
    protected override void End()
    {
        animator.SetBool("active", false);
        gameObject.SetActive(false);
    }
    protected override void Behavior()
    {
        Debug.Log(timer);
        transform.position = player.position;
        timer += Time.deltaTime;
    }
}
