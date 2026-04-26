using UnityEngine;

public class DoorPrefab : MonoBehaviour
{
    [SerializeField] private int requiredKeyID;

    [Header("Door Movement")]
    [SerializeField] private Transform doorVisual; // parte que se mueve
    [SerializeField] private float openSpeed = 2f;

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

    public void Open()
    {
        isOpen = true;
        isOpening = true;
        print("Puerta abierta");
        //Animacion: se desliza hacia un costado
    }
}
