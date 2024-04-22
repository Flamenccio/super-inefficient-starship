using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    public class StarFly : MonoBehaviour
    {
        private float turnSpeed;
        private float maxSpeed;
        private float speed = 0f;
        private Transform target;
        private float timer;
        private int loop = 0;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject trail;
        private void Awake()
        {
            maxSpeed = 25;
        }
        public void FlyTo(Transform v)
        {
            speed = maxSpeed;
            target = v;
            rb.rotation = Random.Range(0, 360f);
        }
        private void Update()
        {
            timer += Time.deltaTime;
            turnSpeed = timer / 2f;

            if (target != null)
            {
                float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x);
                rb.rotation = Mathf.LerpAngle(rb.rotation, targetAngle, turnSpeed);
                rb.velocity = transform.right * speed;
            }
            Instantiate(trail, transform.position, Quaternion.identity);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && loop >= 1)
            {
                Destroy(gameObject);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                loop++;
            }
        }
    }
}
