using UnityEngine;
using UnityEngine.Audio;

public class RoomAudioTrigger : MonoBehaviour
{
    public AudioMixerSnapshot roomSnapshot;
    public AudioMixerSnapshot hallwaySnapshot;
    public float transitionTime = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomSnapshot.TransitionTo(transitionTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hallwaySnapshot.TransitionTo(transitionTime);
        }
    }
}