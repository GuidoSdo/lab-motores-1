using UnityEngine;

public class CameraStationPrefab : MonoBehaviour, IInteractable
{
    public enum StationType
    {
        Prepare,
        Security
    }

    public StationType stationType;
    public Texture securityCameraTexture;
    public PlayerMovement playerMovement;

    public AudioSource audioSource;
    public AudioClip turnOnAudio;
    public AudioClip turnOffAudio;

    private bool usingStation = false;

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