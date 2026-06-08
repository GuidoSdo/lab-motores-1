using UnityEngine;

/// <summary>
/// Actualiza las propiedades visuales del material de contorno usado por las camaras de seguridad.
/// </summary>
public class OutlineMaterialEditor : MonoBehaviour
{
    [Tooltip("Renderer que recibe los cambios de color e intensidad del contorno.")]
    [SerializeField] private Renderer outlineRenderer;

    [Header("Color")]
    [Tooltip("Color base aplicado al material de contorno.")]
    [SerializeField] private Color outlineColor = Color.red;

    [Header("Intensity")]
    [Tooltip("Multiplicador de brillo aplicado al color del contorno.")]
    [Range(0f, 10f)]
    [SerializeField] private float intensity = 3f;

    [Tooltip("Fuerza del efecto fresnel aplicado al contorno.")]
    [Range(0f, 10f)]
    [SerializeField] private float fresnelStrength = 3f;

    private MaterialPropertyBlock mpb;

    void Update()
    {
        if (mpb == null)
            mpb = new MaterialPropertyBlock();

        outlineRenderer.GetPropertyBlock(mpb);

        mpb.SetColor("_Color", outlineColor * intensity);
        mpb.SetFloat("_FresnelStrength", fresnelStrength);

        outlineRenderer.SetPropertyBlock(mpb);
    }
}
