using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Controla el nivel de alerta del jugador y transiciona la mezcla de audio segun el estado alcanzado.
/// </summary>
public class AlertController : MonoBehaviour
{

    [Header("Alerta")]
    [Tooltip("Valor actual acumulado de alerta.")]
    [SerializeField] private float currentAlert;

    [Tooltip("Valor maximo que puede alcanzar la alerta.")]
    [SerializeField] private float maxAlert = 100f;

    [Tooltip("Velocidad a la que sube o baja la alerta por segundo.")]
    [SerializeField] private float fillRate = 2f;

    [Tooltip("Duracion de la transicion entre snapshots del mixer.")]
    [SerializeField] private float transitionTime = 1.5f;

    [Header("Audio")]
    [Tooltip("Fuente que reproduce la musica de alerta cuando se activa el primer estado.")]
    [SerializeField] private AudioSource alertMusic;


    [Tooltip("Snapshot usado cuando no hay alerta activa.")]
    [SerializeField] private AudioMixerSnapshot noAlertSnapshot;

    [Tooltip("Snapshot usado al alcanzar el primer nivel de alerta.")]
    [SerializeField] private AudioMixerSnapshot firstAlertSnapshot;

    [Tooltip("Snapshot usado al alcanzar el segundo nivel de alerta.")]
    [SerializeField] private AudioMixerSnapshot SecondAlertSnapshot;

    [Tooltip("Snapshot usado al alcanzar la alerta maxima.")]
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
