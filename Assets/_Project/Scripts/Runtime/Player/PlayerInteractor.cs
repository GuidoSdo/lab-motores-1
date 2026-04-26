using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lanza un raycast desde la camara del jugador para interactuar con objetos del escenario.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("RayCastConfig")]
    [Tooltip("Distancia maxima a la que el jugador puede interactuar con objetos.")]
    [SerializeField] private float _rayLength;

    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _rayLength))
            {
                IInteractable interactuable = hit.collider.GetComponent<IInteractable>();

                if (interactuable != null)
                {
                    interactuable.Interact(gameObject);
                }
            }
        }
    }
}
