using UnityEngine;
using Unity.Cinemachine;

public class GestionnaireCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camVisee;
    [SerializeField] private CinemachineCamera camSuivi;

    private void Start()
    {
        ActiverCamVisee();
    }

    public void ActiverCamVisee()
    {
        camVisee.Priority = 10;
        camSuivi.Priority = 0;
    }

    public void ActiverCamSuivi()
    {
        camVisee.Priority = 0;
        camSuivi.Priority = 10;
    }
}