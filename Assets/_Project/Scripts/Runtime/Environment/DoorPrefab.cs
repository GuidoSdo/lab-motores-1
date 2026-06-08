using UnityEngine;

/// <summary>
/// Controla la apertura de una puerta, validando llave requerida o desbloqueo externo.
/// </summary>
public class DoorPrefab : MonoBehaviour
{
    [Header("Key")]
    [Tooltip("Identificador de la llave necesaria para abrir esta puerta.")]
    [SerializeField] private int requiredKeyID;

    [Header("Door Movement")]
    [Tooltip("Transform visual que se desplaza durante la apertura.")]
    [SerializeField] private Transform doorVisual; // parte que se mueve

    [Tooltip("Velocidad con la que la puerta se mueve hacia la posicion abierta.")]
    [SerializeField] private float openSpeed = 2f;

    [Header("Audio")]
    [Tooltip("Fuente que reproduce el sonido de apertura.")]
    [SerializeField] private AudioSource openDoorAudio;

    [Tooltip("Indica si la puerta requiere llave o puede abrirse por desbloqueo externo.")]
    public bool needsKey = true;

    [Header("Visual")]
    [Tooltip("Mesh que se oculta cuando la puerta termina de abrirse.")]
    [SerializeField] private GameObject doorMesh;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpening = false;
    private bool isOpen = false;

    private void Start()
    {
        if (doorVisual == null)
        {
            Debug.LogError("DoorVisual no asignado en " + name);
            return;
        }

        // Guardamos posición inicial
        closedPosition = doorVisual.localPosition;

        // Calculamos automáticamente el ancho
        Collider col = doorVisual.GetComponent<Collider>();

        if (col != null)
        {
            // Convertimos tamaño del collider a espacio local
            Vector3 localSize = doorVisual.InverseTransformVector(col.bounds.size);

            float width = Mathf.Abs(localSize.x);

            // Se desliza hacia su derecha local
            openPosition = closedPosition + doorVisual.right * width;
        }
        else
        {
            Debug.LogWarning("La puerta no tiene collider en doorVisual");
            openPosition = closedPosition;
        }
    }

    private void Update()
    {
        if (isOpening)
        {
            doorVisual.localPosition = Vector3.MoveTowards(
                doorVisual.localPosition,
                openPosition,
                openSpeed * Time.deltaTime
            );

            // Cuando llega, se detiene
            if (Vector3.Distance(doorVisual.localPosition, openPosition) < 0.01f)
            {
                doorVisual.localPosition = openPosition;
                isOpening = false;
                doorMesh.SetActive(false);
            }
        }
    }

    //Si choca contra el jugador revisa si tiene la llave correcta para abrirse, en tal caso se abre, sino, que le aparezca un mensaje.
    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) return;

        if (other.CompareTag("Player"))
        {
            TryOpen(other.gameObject);
        }
    }

    private void TryOpen(GameObject interactor)
    {
        if (needsKey == true)
        {
            KeyController keys = interactor.GetComponentInParent<KeyController>();

            if (keys == null || !keys.HasKey(requiredKeyID))
            {
                print("Necesitas la llave " + requiredKeyID);
                return;
            }
            //Usa la llave especifica si es que la tiene
            keys.UseKey(requiredKeyID);
            Open();
        }
        else
        {
            Open();
        }
        
    }

    public void Open()
    {
        isOpen = true;
        isOpening = true;
        openDoorAudio.Play();
        //Animacion: se desliza hacia un costado
    }
}
