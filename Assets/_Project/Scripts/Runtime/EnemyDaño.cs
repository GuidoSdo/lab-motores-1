using UnityEngine;

public class EnemyVida : MonoBehaviour
{
    [SerializeField] private int dañoAlPlayer = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerVida player = other.GetComponent<PlayerVida>();
            if (player != null)
            {
                player.RecibirDaño(dañoAlPlayer);
                Debug.Log("El enemigo dañó al jugador con Trigger");
            }
        }
    }
}
