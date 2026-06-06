using UnityEngine;

public class KeySpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject key;
    [SerializeField] private KeyPrefab keyPrefab;
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;

    void Start()
    {
        Transform randomPos = Random.Range(0, 2) == 0 ? pos1 : pos2;

        keyPrefab.SetStartPos(randomPos.position);

        key.transform.position = randomPos.position;
    }
}