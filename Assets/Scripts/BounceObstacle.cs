using UnityEngine;

public class BounceObstacle : MonoBehaviour
{
    [SerializeField] private float bounceForce = 8f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 velocity = rb.linearVelocity;
                velocity.y = 0f;
                rb.linearVelocity = velocity;

                rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            }
        }
    }
}