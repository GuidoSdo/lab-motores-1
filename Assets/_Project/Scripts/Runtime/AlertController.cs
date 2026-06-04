using UnityEngine;
using UnityEngine.Audio;

public class AlertController : MonoBehaviour
{

    [SerializeField] private float currentAlert;
    [SerializeField] private float maxAlert = 100f;

    [SerializeField] private float fillRate = 2f;

    [SerializeField] private float transitionTime = 1.5f;

    [SerializeField] private AudioSource alertMusic;


    [SerializeField] private AudioMixerSnapshot noAlertSnapshot;
    [SerializeField] private AudioMixerSnapshot firstAlertSnapshot;
    [SerializeField] private AudioMixerSnapshot SecondAlertSnapshot;
    [SerializeField] private AudioMixerSnapshot lastAlertSnapshot;

    public float AlertPercent => currentAlert / maxAlert;

    private enum AlertState
    {
        None,
        First,
        Second,
        Last
    }
    private AlertState state = AlertState.None;



    private void Update()
    {
        AlertState newState = GetCurrentState();

        if (newState != state)
        {
            state = newState;
            OnStateChanged(state);
        }
    }

    private AlertState GetCurrentState()
    {
        if (AlertPercent >= 1f)
            return AlertState.Last;

        if (AlertPercent >= 0.7f)
            return AlertState.Second;

        if (AlertPercent >= 0.4f)
            return AlertState.First;

        return AlertState.None;
    }


    private void OnStateChanged(AlertState newState)
    {
        switch (newState)
        {
            case AlertState.None:
                noAlertSnapshot.TransitionTo(transitionTime);
                break;

            case AlertState.First:
                alertMusic.Play();
                firstAlertSnapshot.TransitionTo(transitionTime);
                break;

            case AlertState.Second:
                SecondAlertSnapshot.TransitionTo(transitionTime);
                break;

            case AlertState.Last:
                lastAlertSnapshot.TransitionTo(transitionTime);
                break;
        }
    }


    /*
    audioMixer.SetFloat(
        "MusicVolume",
        Mathf.Lerp(-20f, 0f, alertController.AlertPercent)
    );
    //Si quiero que la alerta vaya bajando
    //LowerAlert(decayRate * Time.deltaTime);
    */

    public void RaiseAlert()
    {
        currentAlert = Mathf.Clamp(
            currentAlert + fillRate * Time.deltaTime,
            0f,
            maxAlert
        );
    }

    public void LowerAlert()
    {
        currentAlert = Mathf.Clamp(
            currentAlert - fillRate * Time.deltaTime,
            0f,
            maxAlert
        );
    }
}
