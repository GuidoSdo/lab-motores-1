using UnityEngine;

public class PlayerVida : MonoBehaviour
{
    [SerializeField] private int maxVida = 100; // Vida máxima editable en Inspector
    private int vidaActual;

    void Start()
    {
        vidaActual = maxVida; // Al iniciar, vida completa
    }

    public void RecibirDaño(int daño)
    {
        vidaActual -= daño;
        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
    }

    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > maxVida)
            vidaActual = maxVida;
    }

    private void Morir()
    {
        Debug.Log("El jugador murió");
        // Acá podés reiniciar nivel, cargar escena, etc.
    }

    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        RecibirDaño(50); 
    }
}

}
