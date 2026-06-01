using UnityEngine;
using UnityEngine.AI;

public class FootstepsAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.5f;

    private NavMeshAgent agent;
    private CharacterController controller;

    private float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool moving = false;

        if (agent != null)
            moving = agent.velocity.magnitude > 0.1f;

        if (controller != null)
            moving = controller.velocity.magnitude > 0.1f;

        if (!moving)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            PlayFootstep();
            timer = stepInterval;
        }
    }

    private void PlayFootstep()
    {
        audioSource.PlayOneShot(
            footstepClips[Random.Range(0, footstepClips.Length)]
        );
    }
}