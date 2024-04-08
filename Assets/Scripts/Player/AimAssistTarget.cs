using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimAssistTarget : MonoBehaviour // this script controls the visuals for aim assist
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public static AimAssistTarget instance;
    private Transform target;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        Hide();
    }
    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
            Hide();
        }
    }
    public void Hide()
    {
        target = null;
    }
    public void Show(Transform target)
    {
        if (this.target != target)
        { 
            this.target = target;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.crosshairsLockon, target.position);
        }
    }

}
