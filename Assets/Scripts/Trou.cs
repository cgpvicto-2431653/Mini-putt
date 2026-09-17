using UnityEngine;

public class Trou : MonoBehaviour
{
    [Header("Points d'apparition")]
    [SerializeField, Tooltip("Transform marquant le point de départ du niveau pour replacer la balle")]
    private Transform pointDepart;

    [Header("Gestion du Drapeau")]
    [SerializeField, Tooltip("GameObject du drapeau à masquer quand la balle approche")]
    private GameObject drapeau;

    [SerializeField, Tooltip("Distance à laquelle le drapeau disparaît")]
    private float distanceMasquerDrapeau = 2.0f;

    [Header("Référence Balle")]
    [SerializeField] private Balle balle;

    private void Update()
    {
        GérerVisibilitéDrapeau();
    }

    private void GérerVisibilitéDrapeau()
    {
        if (drapeau == null || balle == null) return;

        float distance = Vector3.Distance(transform.position, balle.transform.position);

        bool tropProche = distance <= distanceMasquerDrapeau;

        if (drapeau.activeSelf == tropProche)
        {
            drapeau.SetActive(!tropProche);
        }
    }

    private void OnTriggerEnter(Collider autre)
    {
        if (autre.CompareTag("Player") || autre.GetComponent<Balle>() != null)
        {
            Debug.Log("Balle rentrée dans le trou !");
            ReplacerBalle(autre.gameObject);
        }
    }

    private void ReplacerBalle(GameObject objetBalle)
    {
        if (pointDepart == null)
        {
            Debug.LogWarning("Attention : Le Point Depart n'est pas assigné sur " + gameObject.name);
            return;
        }

        objetBalle.transform.position = pointDepart.position;
        objetBalle.transform.rotation = pointDepart.rotation;

        Rigidbody rb = objetBalle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (drapeau != null)
        {
            drapeau.SetActive(true);
        }
    }
}