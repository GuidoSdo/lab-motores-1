using UnityEngine;

/// <summary>
/// Recarga la bateria del jugador cuando se interactua con el pickup.
/// </summary>
public class BatteryPrefab : MonoBehaviour, IInteractable
{
    [Tooltip("Cantidad de bateria que recupera el jugador al recoger este objeto.")]
    [SerializeField] private float charge = 25f;

    public void Interact(GameObject interactor)
    {
        BatteryController battery = interactor.GetComponentInParent<BatteryController>();

        if (battery == null) return;

        battery.Recharge(charge);
        Destroy(gameObject);
    }
}
