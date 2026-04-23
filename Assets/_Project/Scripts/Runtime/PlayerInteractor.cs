using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("RayCastConfig")]
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
