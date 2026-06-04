using UnityEngine;

public class KeyPrefab : MonoBehaviour, IInteractable
{
    [SerializeField] private int keyID;
    [SerializeField] private AudioSource keyPickupSound;
    public void Interact(GameObject interactor)
    {
        KeyController key = interactor.GetComponentInParent<KeyController>();

        if (key == null) return;

        key.AddKey(keyID);
        keyPickupSound.Play();
        Destroy(gameObject);
    }
}
