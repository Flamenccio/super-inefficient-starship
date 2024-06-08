using TMPro;
using UnityEngine;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls the HUD element below the crosshairs. Shows the player's remaining ammo.
    /// </summary>
    public class CrosshairAmmoController : MonoBehaviour
    {
        [SerializeField] private Transform crosshairs;
        [SerializeField] private Camera cam;
        private TMP_Text display;
        private Vector3 offset = new(0f, -50f);

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
}