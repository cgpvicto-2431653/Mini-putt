using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gère les déplacements et rotations de la cible de caméra Cinemachine.
/// </summary>
public class CibleCamera : MonoBehaviour
{
    [Header("Paramètres de déplacement")]
    [SerializeField, Tooltip("Vitesse de déplacement en m/s")]
    private float vitesseDeplacement;

    [SerializeField, Tooltip("Vitesse de rotation en deg/sec")]
    private float vitesseRotation;

    [SerializeField, Tooltip("Vitesse inclinaison en deg/sec")]
    private float vitesseInclinaison;

    [SerializeField, Tooltip("Angles d'inclinaison limites de la caméra. Doit être plus petit que le premier angle ou plus grand que le second")]
    private Vector2 limitesInclinaison;

    [SerializeField, Tooltip("Vitesse zoom de la caméra")]
    private float vitesseZoom;

    [SerializeField, Tooltip("Limites du zoom de la caméra")]
    private Vector2 limitesZoom;

    [Header("Références aux objets de jeu")]
    [SerializeField, Tooltip("Le PlayerInput qui gère les actions de la personne qui joue")]
    private PlayerInput controles;

    [SerializeField, Tooltip("Zone de confinement de la caméra")]
    private BoxCollider volumeCamera;

    [SerializeField, Tooltip("La caméra qui suit la cible")]
    private CinemachineCamera cameraGeree;

    private Vector2 deplacement;

    private float rotation;

    private float inclinaison;

    private float zoom;

    private void Start()
    {
        // Auto-récupération du PlayerInput si non assigné dans l'Inspecteur
        if (controles == null)
        {
            controles = GetComponent<PlayerInput>();
        }

        if (controles == null || controles.actions == null)
        {
            Debug.LogError("PlayerInput manquant ou non configuré sur " + gameObject.name);
            return;
        }

        // Action de déplacement (WASD)
        InputAction actionDeplacement = controles.actions.FindAction("player/DeplacerCamera");
        if (actionDeplacement != null)
        {
            actionDeplacement.performed += CommencerDeplacement;
            actionDeplacement.canceled += TerminerDeplacement;
        }

        // Action de rotation Y (Q / E)
        InputAction actionRotation = controles.actions.FindAction("player/TournerCamera");
        if (actionRotation != null)
        {
            actionRotation.performed += CommencerRotation;
            actionRotation.canceled += TerminerRotation;
        }

        // Action d'inclinaison X (Z / X)
        InputAction actionInclinaison = controles.actions.FindAction("player/InclinerCamera");
        if (actionInclinaison != null)
        {
            actionInclinaison.performed += CommencerInclinaison;
            actionInclinaison.canceled += TerminerInclinaison;
        }

        // Action de Zoom (Molette de la souris)
        InputAction actionZoom = controles.actions.FindAction("player/ZoomerCamera");
        if (actionZoom != null)
        {
            actionZoom.performed += CommencerZoom;
            actionZoom.canceled += TerminerZoom;
        }
    }

    private void Update()
    {
        DeplacerCamera();
        TournerCamera();
        InclinerCamera();
        ZoomerCamera();
    }

    private void OnDestroy()
    {
        if (controles == null || controles.actions == null) { return; }

        // Retire les callbacks des actions pour éviter les fuites de mémoire
        InputAction actionDeplacement = controles.actions.FindAction("player/DeplacerCamera");
        actionDeplacement.performed -= CommencerDeplacement;
        actionDeplacement.canceled -= TerminerDeplacement;

        InputAction actionRotation = controles.actions.FindAction("player/TournerCamera");
        actionRotation.performed -= CommencerRotation;
        actionRotation.canceled -= TerminerRotation;

        InputAction actionInclinaison = controles.actions.FindAction("player/InclinerCamera");
        actionInclinaison.performed -= CommencerInclinaison;
        actionInclinaison.canceled -= TerminerInclinaison;

        InputAction actionZoom = controles.actions.FindAction("player/ZoomerCamera");
        actionZoom.performed -= CommencerZoom;
        actionZoom.canceled -= TerminerZoom;
    }

    #region Déplacement
    private void CommencerDeplacement(InputAction.CallbackContext contexte)
    {
        deplacement = vitesseDeplacement * contexte.ReadValue<Vector2>();
    }

    private void TerminerDeplacement(InputAction.CallbackContext contexte)
    {
        deplacement = Vector2.zero;
    }

    private void DeplacerCamera()
    {
        if (deplacement.sqrMagnitude > 0.0f)
        {
            Vector3 prochainePosition = transform.position +
                transform.right * deplacement.x * Time.deltaTime +
                transform.forward * deplacement.y * Time.deltaTime;
            prochainePosition = Vector3.Scale(prochainePosition, new Vector3(1.0f, 0.0f, 1.0f));

            if (volumeCamera == null || volumeCamera.bounds.Contains(prochainePosition))
            {
                transform.position = prochainePosition;
            }
        }
    }
    #endregion

    #region Rotation (Axe Y - Q/E)
    private void CommencerRotation(InputAction.CallbackContext contexte)
    {
        rotation = vitesseRotation * contexte.ReadValue<float>();
    }

    private void TerminerRotation(InputAction.CallbackContext contexte)
    {
        rotation = 0.0f;
    }

    private void TournerCamera()
    {
        if (Mathf.Abs(rotation) > 0.01f)
        {
            transform.Rotate(new Vector3(0.0f, rotation * Time.deltaTime, 0.0f), Space.World);
        }
    }
    #endregion

    #region Inclinaison (Axe X - Z/X)

    private void CommencerInclinaison(InputAction.CallbackContext contexte)
    {
        inclinaison = vitesseInclinaison * contexte.ReadValue<float>();
    }

    private void TerminerInclinaison(InputAction.CallbackContext contexte)
    {
        inclinaison = 0.0f;
    }

    private void InclinerCamera()
    {
        float angle = (transform.localEulerAngles.x + inclinaison * Time.deltaTime) % 360;

        if (angle < limitesInclinaison.x || angle > limitesInclinaison.y)
        {
            transform.Rotate(new Vector3(inclinaison * Time.deltaTime, 0.0f, 0.0f), Space.Self);
        }
    }
    #endregion

    #region Zoom
    private void CommencerZoom(InputAction.CallbackContext contexte)
    {
        float sensScroll = Mathf.Sign(contexte.ReadValue<float>());


        CinemachinePositionComposer positionComposer = cameraGeree.GetComponent<CinemachinePositionComposer>();
        if (positionComposer == null) return;

        Vector3 direction = positionComposer.TargetOffset.normalized;

        float distanceActuelle = positionComposer.TargetOffset.magnitude;
        float nouvelleDistance = distanceActuelle - (sensScroll * vitesseZoom);

        nouvelleDistance = Mathf.Clamp(nouvelleDistance, limitesZoom.x, limitesZoom.y);

        positionComposer.TargetOffset = direction * nouvelleDistance;
    }

    private void TerminerZoom(InputAction.CallbackContext contexte)
    {
        zoom = 0.0f;
    }

    private void ZoomerCamera()
    {
        CinemachinePositionComposer positionComposer = cameraGeree.GetComponent<CinemachinePositionComposer>();
        Vector3 offsetCamera = positionComposer.TargetOffset + positionComposer.TargetOffset.normalized * zoom;
        float distanceCamera = offsetCamera.magnitude;

        if (distanceCamera >= limitesZoom.x && distanceCamera <= limitesZoom.y)
        {
            positionComposer.TargetOffset = offsetCamera;
        }
    }
    #endregion
}