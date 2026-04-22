using UnityEngine;

public class Battery : MonoBehaviour, IInteractable
{
    public float charge = 25f;
    public void Interact(GameObject interactor)
    {
        BatteryController battery = interactor.GetComponentInParent<BatteryController>();

        if (battery == null) return;

        battery.Recharge(charge);
        Destroy(gameObject);
    }
}
