using UnityEngine;

/// <summary>
/// Ubica la llave inicial en una de las posiciones posibles de la escena.
/// </summary>
public class KeySpawnerController : MonoBehaviour
{
    [Tooltip("Objeto de llave que se reposiciona al iniciar la escena.")]
    [SerializeField] private GameObject key;

    [Tooltip("Componente de la llave que recibe la posicion base para su animacion.")]
    [SerializeField] private KeyPrefab keyPrefab;

    [Tooltip("Primera posicion posible para generar la llave.")]
    [SerializeField] private Transform pos1;

    [Tooltip("Segunda posicion posible para generar la llave.")]
    [SerializeField] private Transform pos2;

    private void Start()
    {
        Transform randomPos = Random.Range(0, 2) == 0 ? pos1 : pos2;

        keyPrefab.SetStartPos(randomPos.position);

        key.transform.position = randomPos.position;
    }
}
