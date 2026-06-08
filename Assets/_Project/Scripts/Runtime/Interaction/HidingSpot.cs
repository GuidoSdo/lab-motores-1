using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Oculta al jugador de los enemigos y cancela cualquier persecucion activa.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class HidingSpot : MonoBehaviour
{
    [Header("Uso")]
    [Tooltip("Si esta activo, el escondite queda inutilizado despues de que el jugador sale por primera vez.")]
    [SerializeField] private bool singleUse = true;

    [Tooltip("Segundos que el jugador permanece oculto antes de salir automaticamente.")]
    [Min(0.1f)]
    [SerializeField] private float hidingDuration = 5f;

    [Header("Depuracion")]
    [Tooltip("Muestra logs al detectar al jugador, iniciar el escondite, actualizar el temporizador y finalizar.")]
    [SerializeField] private bool enableDebugLogs = true;

    [SerializeField] private ObjectiveController objectiveController;

    private Collider interactionTrigger;
    private GameObject playerInRange;
    private PlayerController hiddenPlayer;
    private PlayerMovement playerMovement;
    private CharacterController characterController;
    private Renderer[] playerRenderers;
    private bool[] rendererStates;
    private bool movementWasEnabled;
    private bool characterControllerWasEnabled;
    private bool isPlayerHidden;
    private bool isConsumed;
    private float remainingHidingTime;
    private int lastLoggedSecond = -1;

    private bool usedHidingSpot = false;

    private void Awake()
    {
        interactionTrigger = GetComponent<Collider>();
        if (hidingDuration <= 0f)
        {
            hidingDuration = 5f;
        }

        ValidateSetup();
    }

    private void OnValidate()
    {
        if (hidingDuration <= 0f)
        {
            hidingDuration = 5f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isConsumed)
        {
            return;
        }

        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        playerInRange = playerController.gameObject;
        if (playerInRange && usedHidingSpot == false)
        {
            objectiveController.AddObjective("Pulsa [E] para esconderte al estar sobre piso morado (Evita persecución, un único uso)");
            usedHidingSpot = true;
        }
        LogDebug($"Jugador '{playerInRange.name}' dentro del rango. Presionar E para esconderse.");
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController != null &&
            playerController.gameObject == playerInRange &&
            !isPlayerHidden)
        {
            LogDebug($"Jugador '{playerInRange.name}' salio del rango.");
            playerInRange = null;
        }
    }

    private void Update()
    {
        if (isPlayerHidden)
        {
            remainingHidingTime -= Time.deltaTime;
            LogRemainingHidingTime();

            if (remainingHidingTime <= 0f)
            {
                ExitHidingSpot();
            }

            return;
        }

        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
        {
            return;
        }

        if (!isConsumed && playerInRange != null)
        {
            EnterHidingSpot(playerInRange);
        }
    }

    private void EnterHidingSpot(GameObject player)
    {
        hiddenPlayer = player.GetComponent<PlayerController>();
        if (hiddenPlayer == null)
        {
            Debug.LogError(
                $"[{nameof(HidingSpot)}] El jugador necesita {nameof(PlayerController)} para usar '{name}'.",
                this);
            return;
        }

        playerMovement = player.GetComponent<PlayerMovement>();
        characterController = player.GetComponent<CharacterController>();
        playerRenderers = player.GetComponentsInChildren<Renderer>();

        movementWasEnabled = playerMovement != null && playerMovement.enabled;
        characterControllerWasEnabled =
            characterController != null && characterController.enabled;

        rendererStates = new bool[playerRenderers.Length];
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            rendererStates[i] = playerRenderers[i].enabled;
            playerRenderers[i].enabled = false;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        hiddenPlayer.SetHidden(true);
        isPlayerHidden = true;
        isConsumed = singleUse;
        remainingHidingTime = Mathf.Max(0.1f, hidingDuration);
        lastLoggedSecond = -1;
        LogDebug(
            $"Jugador '{hiddenPlayer.name}' oculto durante {remainingHidingTime:0.##} segundos.");
        LogRemainingHidingTime();

        AbortEnemyPursuit();
    }

    private void ExitHidingSpot()
    {
        hiddenPlayer.SetHidden(false);
        LogDebug($"Finalizo el escondite de '{hiddenPlayer.name}'.");

        if (playerMovement != null)
        {
            playerMovement.enabled = movementWasEnabled;
        }

        if (characterController != null)
        {
            characterController.enabled = characterControllerWasEnabled;
        }

        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null)
            {
                playerRenderers[i].enabled = rendererStates[i];
            }
        }

        isPlayerHidden = false;
        playerInRange = null;

        if (isConsumed)
        {
            interactionTrigger.enabled = false;
            enabled = false;
        }
    }

    private void LogRemainingHidingTime()
    {
        int remainingSeconds = Mathf.CeilToInt(Mathf.Max(remainingHidingTime, 0f));
        if (remainingSeconds == lastLoggedSecond)
        {
            return;
        }

        lastLoggedSecond = remainingSeconds;
        LogDebug($"'{hiddenPlayer.name}': {remainingSeconds} segundo(s) restantes.");
    }

    private void AbortEnemyPursuit()
    {
        EnemyBehaviour[] enemies =
            FindObjectsByType<EnemyBehaviour>(FindObjectsSortMode.None);

        int abortedPursuits = 0;
        foreach (EnemyBehaviour enemy in enemies)
        {
            if (enemy.AbortPursuit())
            {
                abortedPursuits++;
            }
        }

        LogDebug($"Persecucion cancelada para {abortedPursuits} enemigo(s).");
    }

    private void ValidateSetup()
    {
        if (!interactionTrigger.isTrigger)
        {
            Debug.LogError(
                $"[{nameof(HidingSpot)}] El Collider de '{name}' debe tener Is Trigger activado.",
                this);
            enabled = false;
        }
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{nameof(HidingSpot)}] {message}", this);
        }
    }
}
