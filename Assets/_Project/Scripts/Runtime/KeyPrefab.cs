using UnityEngine;

public class KeyPrefab : MonoBehaviour, IInteractable
{
    [SerializeField] private int keyID;
    [SerializeField] private AudioSource keyPickupSound;
    [SerializeField] private ObjectiveController objectiveController;
    [SerializeField] private bool isObjective;
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
