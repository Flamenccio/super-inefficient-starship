using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrosshairAmmoController : MonoBehaviour
{
    private TMP_Text display;
    [SerializeField] private Transform crosshairs;
    private Vector3 offset = new Vector3(0f, -50f);
    [SerializeField] private Camera cam;
    private void Start()
    {
        display = gameObject.GetComponent<TMP_Text>();
    }
    public void UpdateAmmo(int ammo)
    {
        display.text = ammo.ToString();
    }
    private void FixedUpdate()
    {
        transform.position = cam.WorldToScreenPoint(crosshairs.position) + offset;
    }
}