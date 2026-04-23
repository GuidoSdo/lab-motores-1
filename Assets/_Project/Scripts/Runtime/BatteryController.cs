using UnityEngine;

public class BatteryController : MonoBehaviour
{

    [SerializeField] private float currentBattery;
    [SerializeField] private float maxBattery = 100f;
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
