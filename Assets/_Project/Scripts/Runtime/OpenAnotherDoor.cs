using UnityEngine;

public class OpenAnotherDoor : MonoBehaviour
{

    [SerializeField] private DoorPrefab door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Open();
        }
    }
}
