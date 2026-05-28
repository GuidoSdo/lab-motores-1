using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HidingSpot : MonoBehaviour
{
    [Header("Puntos de Referencia")]
    [SerializeField] private Transform cameraTargetPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float transitionSpeed = 4f;

    [Header("Interfaz de Usuario")]
    [Tooltip("Arrastrá acá el panel, texto o GameObject de la UI de la batería/tablet para ocultarlo al esconderse.")]
    [SerializeField] private GameObject playerUItoHide;

    private bool isPlayerInside = false;
    private bool isTransitioning = false;

    // --- 🌟 NUEVO: CONTROL DE RANGO EXCLUSIVO 🌟 ---
    private bool isPlayerInTrigger = false;
    private GameObject detectedPlayer;
    // ------------------------------------------------

    private Transform playerTransform;
    private Transform playerCameraTransform;
    private Vector3 originalCameraLocalPos;
    private Quaternion originalCameraLocalRot;

    private PlayerMovement playerMovementScript;
    private Keyboard currentKeyboard;

    private Renderer[] playerRenderers;

    private void Start()
    {
        currentKeyboard = Keyboard.current;
    }

    // 🌟 MEJORA: El Trigger ahora SOLO prende o apaga el permiso de interactuar
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            detectedPlayer = other.gameObject;

            // Acá podés prender un cartelito de UI que diga "Presioná E para esconderte"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            detectedPlayer = null;

            // Acá podés apagar el cartelito de UI
        }
    }

    private void Update()
    {
        if (isTransitioning || currentKeyboard == null) return;

        // Escuchamos la tecla E a la velocidad de los FPS
        if (currentKeyboard.eKey.wasPressedThisFrame)
        {
            if (!isPlayerInside && isPlayerInTrigger && detectedPlayer != null)
            {
                StartCoroutine(EnterHideout(detectedPlayer));
            }
            else if (isPlayerInside)
            {
                StartCoroutine(ExitHideout());
            }
        }
    }

    private IEnumerator EnterHideout(GameObject player)
    {
        isTransitioning = true;

        playerTransform = player.transform;
        playerCameraTransform = player.GetComponentInChildren<Camera>().transform;

        originalCameraLocalPos = playerCameraTransform.localPosition;
        originalCameraLocalRot = playerCameraTransform.localRotation;

        playerMovementScript = player.GetComponent<PlayerMovement>();

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Apagar el cuerpo del jugador
        playerRenderers = player.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in playerRenderers)
        {
            if (rend.gameObject != playerCameraTransform.gameObject)
            {
                rend.enabled = false;
            }
        }

        if (playerUItoHide != null)
        {
            playerUItoHide.SetActive(false);
        }

        Vector3 startPos = playerCameraTransform.position;
        Quaternion startRot = playerCameraTransform.rotation;

        playerCameraTransform.SetParent(null);

        playerTransform.position = exitPoint.position;
        playerTransform.rotation = exitPoint.rotation;

        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            playerCameraTransform.position = Vector3.Lerp(startPos, cameraTargetPoint.position, elapsed);
            playerCameraTransform.rotation = Quaternion.Lerp(startRot, cameraTargetPoint.rotation, elapsed);
            yield return null;
        }

        playerCameraTransform.position = cameraTargetPoint.position;
        playerCameraTransform.rotation = cameraTargetPoint.rotation;
        playerCameraTransform.SetParent(cameraTargetPoint);

        isPlayerInside = true;
        isTransitioning = false;
    }

    private IEnumerator ExitHideout()
    {
        isTransitioning = true;

        float elapsed = 0f;
        Vector3 startPos = playerCameraTransform.position;
        Quaternion startRot = playerCameraTransform.rotation;

        playerCameraTransform.SetParent(null);

        Vector3 targetWorldPos = exitPoint.position + (exitPoint.rotation * originalCameraLocalPos);
        Quaternion targetWorldRot = exitPoint.rotation;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            playerCameraTransform.position = Vector3.Lerp(startPos, targetWorldPos, elapsed);
            playerCameraTransform.rotation = Quaternion.Lerp(startRot, targetWorldRot, elapsed);
            yield return null;
        }

        playerCameraTransform.SetParent(playerTransform);
        playerCameraTransform.localPosition = originalCameraLocalPos;
        playerCameraTransform.localRotation = Quaternion.identity;

        if (playerRenderers != null)
        {
            foreach (Renderer rend in playerRenderers)
            {
                if (rend != null) rend.enabled = true;
            }
        }

        if (playerUItoHide != null)
        {
            playerUItoHide.SetActive(true);
        }

        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        isPlayerInside = false;
        isTransitioning = false;
    }
}
