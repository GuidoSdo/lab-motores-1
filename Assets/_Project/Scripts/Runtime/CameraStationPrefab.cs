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

    [Header("Unlock")]
    [Tooltip("Puerta que se desbloquea cuando se usa una estacion de preparacion.")]
    public DoorPrefab doorToUnlock;

    public ObjectiveController objectiveController;

    public void Interact(GameObject interactor)
    {
        ScreenController screen = interactor.GetComponentInChildren<ScreenController>();

        if (screen == null)
            return;

        bool changed = false;

        switch (stationType)
        {
            case StationType.Prepare:
                changed = screen.ActivateCamera();
                if (changed)
                {
                    if (screen.IsScreenOpen)
                    {
                        audioSource.PlayOneShot(turnOnAudio);
                        objectiveController.CompleteObjective("- Usar la estación [E] para desbloquear la puerta");
                        doorToUnlock.needsKey = false;
                    }
                    else
                    {
                        audioSource.PlayOneShot(turnOffAudio);
                    }
                }
                break;

            case StationType.Security:
                changed = screen.ActivateSecurityCamera(
                    securityCameraTexture);
                if (changed)
                {
                    if (screen.IsScreenOpen)
                    {
                        audioSource.PlayOneShot(turnOnAudio);
                        objectiveController.CompleteObjective("- Encuentra la estación de vigilancia");
                    }
                    else
                    {
                        audioSource.PlayOneShot(turnOffAudio);
                    }
                }

                break;
        }

        // Si la pantalla estaba en cooldown no hacemos nada
        if (!changed)
            return;

        // El movimiento depende del estado real de la pantalla
        playerMovement._canControl = !screen.IsScreenOpen;
    }
}