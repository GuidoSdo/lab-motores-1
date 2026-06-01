using UnityEngine;
using UnityEngine.Audio;

public class AlertController : MonoBehaviour
{

    [SerializeField] private float currentAlert;
    [SerializeField] private float maxAlert = 100f;

    [SerializeField] private float fillRate = 2f;

    public float AlertPercent => currentAlert / maxAlert;
    
    private void Update()
    {
        if(AlertPercent == 0.4f)
        {
            print("30%");
        }
        /*
        if (AlertPercent > 0.7f)
        {
            dangerSnapshot.TransitionTo(1f);
        }
        */

        /*
        audioMixer.SetFloat(
            "MusicVolume",
            Mathf.Lerp(-20f, 0f, alertController.AlertPercent)
        );
        */



        //Si quiero que la alerta vaya bajando
        //LowerAlert(decayRate * Time.deltaTime);
    }


    public void RaiseAlert(float amount)
    {
        currentAlert = Mathf.Clamp(
            currentAlert + amount,
            0f,
            maxAlert
        );
    }

    public void LowerAlert(float amount)
    {
        currentAlert = Mathf.Clamp(
            currentAlert - amount,
            0f,
            maxAlert
        );
    }
}
