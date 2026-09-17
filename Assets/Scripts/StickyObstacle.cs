using UnityEngine;

public class StickyObstacle : MonoBehaviour
{
    [SerializeField] private float dragFactor = 5f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * dragFactor);
            }
        }
    }
}