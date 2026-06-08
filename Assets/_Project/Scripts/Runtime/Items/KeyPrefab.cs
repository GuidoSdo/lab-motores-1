using UnityEngine;

/// <summary>
/// Representa una llave recolectable e informa al inventario del jugador cuando se interactua con ella.
/// </summary>
public class KeyPrefab : MonoBehaviour, IInteractable
{
    [Tooltip("Identificador de la llave que se agrega al inventario.")]
    [SerializeField] private int keyID;

    [Tooltip("AudioSource que reproduce el sonido de recoleccion.")]
    [SerializeField] private AudioSource keyPickupSound;

    [Tooltip("Controlador de objetivos que se actualiza si esta llave completa un objetivo.")]
    [SerializeField] private ObjectiveController objectiveController;

    [Tooltip("Indica si recolectar esta llave completa el objetivo asociado.")]
    [SerializeField] private bool isObjective;

    [Tooltip("Velocidad de rotacion visual de la llave.")]
    [SerializeField] private float rotationSpeed = 90f;

    [Tooltip("Altura maxima de la oscilacion vertical.")]
    [SerializeField] private float floatAmplitude = 0.25f;

    [Tooltip("Velocidad de la oscilacion vertical.")]
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    public void SetStartPos(Vector3 startPos)
    {
        startPosition = startPos;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPosition + Vector3.up * yOffset;
    }


    public void Interact(GameObject interactor)
    {
        KeyController key = interactor.GetComponentInParent<KeyController>();

        if (key == null) return;
        if (isObjective)
        {
            objectiveController.CompleteObjective("- Encuentra la llave para escapar");
        }

        key.AddKey(keyID);
        keyPickupSound.Play();
        Destroy(gameObject);
    }
}
