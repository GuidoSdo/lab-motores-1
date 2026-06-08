using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mantiene el inventario de llaves del jugador por identificador.
/// </summary>
public class KeyController : MonoBehaviour
{
    private readonly Dictionary<int, int> keys = new();

    public void AddKey(int keyID)
    {
        if (keys.ContainsKey(keyID))
            keys[keyID]++;
        else
            keys[keyID] = 1;
        print("Agarrada la llave: " + keyID);
    }

    public bool HasKey(int keyID)
    {
        return keys.ContainsKey(keyID) && keys[keyID] > 0;
    }
    public void UseKey(int keyID)
    {
        if (!HasKey(keyID)) return;

        keys[keyID]--;

        if (keys[keyID] <= 0)
            keys.Remove(keyID);
    }
}
