using UnityEngine;

/// <summary>
/// Administra la energia disponible para sistemas del jugador que consumen bateria.
/// </summary>
public class BatteryController : MonoBehaviour
{
    [Tooltip("Carga actual de la bateria.")]
    [SerializeField] private float currentBattery;

    [Tooltip("Carga maxima posible de la bateria.")]
    [SerializeField] private float maxBattery = 100f;

    [Tooltip("Consumo base por segundo usado al drenar la bateria.")]
    [SerializeField] private float drainSpeed = 5f;

    public float BatteryPercent => currentBattery / maxBattery;
    public bool HasBattery => currentBattery > 0f;

    

    public void Recharge(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0, maxBattery);
    }

    public void Consume(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery - amount, 0, maxBattery);
    }

    public void Drain(float multiplier)
    {
        Consume(drainSpeed * multiplier * Time.deltaTime);
    }
}
