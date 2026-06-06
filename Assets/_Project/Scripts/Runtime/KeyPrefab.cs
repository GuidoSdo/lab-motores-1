using UnityEngine;

public class KeyPrefab : MonoBehaviour, IInteractable
{
    [SerializeField] private int keyID;
    [SerializeField] private AudioSource keyPickupSound;
    [SerializeField] private ObjectiveController objectiveController;
    [SerializeField] private bool isObjective;

    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    public void SetStartPos(Vector3 startPos)
    {
        startPosition = startPos;
    }

    private void Update()
    {
        // Rotación
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Flotación
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
