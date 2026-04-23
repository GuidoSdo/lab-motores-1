using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    //Guarda las llaves que tiene el jugador actualmente
    //Se guarda en un diccionario en caso de que haya multiples tipos o cantidades de llaves
    //mas escalable
    private Dictionary<int, int> keys = new Dictionary<int, int>();

    //Agarra una llave
    public void AddKey(int keyID)
    {
        if (keys.ContainsKey(keyID))
            keys[keyID]++;
        else
            keys[keyID] = 1;
        print("Agarrada la llave: "+ keyID);
    }
    //Revisa si tiene x llave en el inventario
    public bool HasKey(int keyID)
    {
        return keys.ContainsKey(keyID) && keys[keyID] > 0;
    }
    //Remueve la llave del diccionario si es usada
    public void UseKey(int keyID)
    {
        if (!HasKey(keyID)) return;

        keys[keyID]--;

        if (keys[keyID] <= 0)
            keys.Remove(keyID);
    }
}