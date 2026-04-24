using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Punto central del prefab del jugador.
/// Reune referencias a los componentes principales para que otros sistemas
/// puedan identificar al jugador sin acoplarse a un script especifico.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerInteractor interactor;

    public PlayerMovement Movement => movement;
    public PlayerInteractor Interactor => interactor;

    private void Awake()
    {
        ValidatePlayerLayer();

        // Buscamos los componentes en el mismo objeto para mantener el prefab conectado
        // aunque las referencias no hayan sido asignadas manualmente en el inspector.
        if (movement == null)
        {
            movement = GetComponent<PlayerMovement>();
        }

        if (interactor == null)
        {
            interactor = GetComponent<PlayerInteractor>();
        }

    }

    /// <summary>
    /// Verifica que el prefab del jugador este configurado en la layer correcta
    /// para que otros sistemas, como la deteccion de enemigos, puedan encontrarlo.
    /// </summary>
    private void ValidatePlayerLayer()
    {
        int playerLayer = LayerMask.NameToLayer("Player");

        if (playerLayer == -1)
        {
            Debug.LogWarning("La layer 'Player' no existe en el proyecto.");
            return;
        }

        if (gameObject.layer != playerLayer)
        {
            Debug.LogWarning($"[{name}] deberia estar asignado a la layer 'Player'.", this);
        }
    }
}
