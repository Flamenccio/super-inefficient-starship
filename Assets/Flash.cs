using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteren;
    public float Frequency = 0.1f;
    private float timer;
    private bool active = false;
    private Color transparent = new(1f, 1f, 1f, 0f);

    private void Update()
    {
        /*
        if (!active) return;

        if (timer >= Frequency - (3f / 60f) && timer < Frequency)
        {
            spriteren.enabled = true;
            //spriteren.color = Color.white;
        }

        if (timer >= Frequency)
        {
            spriteren.enabled = false;
            //spriteren.color = transparent;
            timer = 0f;
        }

        timer += Time.deltaTime;
        */
    }
    public void Show()
    {
        active = true;
        spriteren.enabled = true;
        //spriteren.color = Color.white;
    }
    public void Hide()
    {
        active = false;
        spriteren.enabled = false;
        //spriteren.color = transparent;
    }
}
