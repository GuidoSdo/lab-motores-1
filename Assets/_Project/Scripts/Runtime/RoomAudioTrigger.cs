using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class RoomAudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private string volumeParameter = "AmbientVolume";
    [SerializeField] private string pitchParameter = "AmbientPitch";

    [SerializeField] private float transitionTime = 1.5f;

    private Coroutine transitionCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StartTransition(
            targetVolume: -5.5f,
            targetPitch: 0.6f
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StartTransition(
            targetVolume: 0f,
            targetPitch: 1f
        );
    }

    private void StartTransition(float targetVolume, float targetPitch)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(
            TransitionAudio(targetVolume, targetPitch)
        );
    }

    private IEnumerator TransitionAudio(
        float targetVolume,
        float targetPitch)
    {
        mixer.GetFloat(volumeParameter, out float startVolume);
        mixer.GetFloat(pitchParameter, out float startPitch);

        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / transitionTime;

            mixer.SetFloat(
                volumeParameter,
                Mathf.Lerp(startVolume, targetVolume, t));

            mixer.SetFloat(
                pitchParameter,
                Mathf.Lerp(startPitch, targetPitch, t));

            yield return null;
        }

        mixer.SetFloat(volumeParameter, targetVolume);
        mixer.SetFloat(pitchParameter, targetPitch);
    }
}