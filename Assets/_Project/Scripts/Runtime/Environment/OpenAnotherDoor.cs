using UnityEngine;

/// <summary>
/// Abre una puerta asociada cuando el jugador entra en el trigger.
/// </summary>
public class OpenAnotherDoor : MonoBehaviour
{
    [Tooltip("Puerta que se abre cuando el jugador activa este trigger.")]
    [SerializeField] private DoorPrefab door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Open();
        }
    }
}
