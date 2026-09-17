using UnityEngine;

public class BoostObstacle : MonoBehaviour
{
    [SerializeField] private float boostForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-transform.forward * boostForce, ForceMode.VelocityChange);
            }
        }
    }
}