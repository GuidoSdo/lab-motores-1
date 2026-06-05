using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Reproduce clips de pasos mientras el objeto se desplaza con NavMeshAgent o CharacterController.
/// </summary>
public class FootstepsAudio : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Fuente desde la que se reproducen los sonidos de pasos.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Clips disponibles para variar el sonido de cada paso.")]
    [SerializeField] private AudioClip[] footstepClips;

    [Tooltip("Tiempo entre reproducciones de pasos mientras el objeto esta en movimiento.")]
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
