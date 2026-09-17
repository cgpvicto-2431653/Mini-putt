using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Balle : MonoBehaviour
{
    [Header("Paramètres du coup")]
    [SerializeField, Tooltip("Force appliquée à la balle (45g)")]
    private float forceTir = 0.5f;

    [SerializeField, Tooltip("Seuil sous lequel la balle est considérée comme arrêtée")]
    private float seuilArret = 0.15f;

    [Header("Références")]
    [SerializeField] private GestionnaireCamera gestionnaireCamera;
    [SerializeField] private Transform cibleCamera;

    private Rigidbody rb;
    private bool estEnMouvement = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 0.045f;
    }

    private void Update()
    {
        bool balleEstImmobile = rb.linearVelocity.magnitude < seuilArret;

        if (Input.GetKeyDown(KeyCode.Space) && !estEnMouvement && balleEstImmobile)
        {
            Tirer();
        }

        if (estEnMouvement && balleEstImmobile)
        {
            ArreterBalle();
        }
    }

    private void Tirer()
    {
        rb.WakeUp();

        Vector3 directionTir = transform.forward;
        if (cibleCamera != null)
        {
            directionTir = Vector3.Scale(cibleCamera.forward, new Vector3(1.0f, 0.0f, 1.0f)).normalized;
        }

        rb.AddForce(directionTir * forceTir, ForceMode.Impulse);
        estEnMouvement = true;

        if (gestionnaireCamera != null)
        {
            gestionnaireCamera.ActiverCamSuivi();
        }
    }

    private void ArreterBalle()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        estEnMouvement = false;

        if (cibleCamera != null)
        {
            cibleCamera.position = transform.position;
        }

        if (gestionnaireCamera != null)
        {
            gestionnaireCamera.ActiverCamVisee();
        }
    }
}