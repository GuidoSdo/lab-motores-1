using UnityEngine;

/// <summary>
/// Maneja la interaccion con estaciones de camara y aplica sus efectos sobre pantalla, audio, puerta y movimiento.
/// </summary>
public class CameraStationPrefab : MonoBehaviour, IInteractable
{
    public enum StationType
    {
        Prepare,
        Security
    }

    [Header("Station")]
    [Tooltip("Define si la estacion prepara el sistema o muestra la camara de seguridad.")]
    public StationType stationType;

    [Tooltip("Textura que se muestra cuando la estacion usa la camara de seguridad.")]
    public Texture securityCameraTexture;

    [Tooltip("Movimiento del jugador que se desactiva mientras usa la estacion.")]
    public PlayerMovement playerMovement;

    [Header("Audio")]
    [Tooltip("Fuente que reproduce los sonidos de encendido y apagado de la estacion.")]
    public AudioSource audioSource;

    [Tooltip("Clip reproducido al empezar a usar la estacion.")]
    public AudioClip turnOnAudio;

    [Tooltip("Clip reproducido al dejar de usar la estacion.")]
    public AudioClip turnOffAudio;

    private bool usingStation = false;

    [Header("Unlock")]
    [Tooltip("Puerta que se desbloquea cuando se usa una estacion de preparacion.")]
    public DoorPrefab doorToUnlock;

    public void Interact(GameObject interactor)
    {
        usingStation = !usingStation;
        if (usingStation)
        {
            audioSource.PlayOneShot(turnOnAudio);
        }
        else 
        {
            audioSource.PlayOneShot(turnOffAudio);
        }

        ScreenController screen = interactor.GetComponentInChildren<ScreenController>();
        if (screen == null) return;

        switch (stationType)
        {
            case StationType.Prepare:
                screen.ActivateCamera();
                doorToUnlock.needsKey = false;
                break;
            case StationType.Security:
                screen.ActivateSecurityCamera(securityCameraTexture);
                break;
        }

        //Desactivo/Activo el movimiento del player cuando interactúa con un panel de cámara
        playerMovement._canControl = !usingStation;
    }
}
