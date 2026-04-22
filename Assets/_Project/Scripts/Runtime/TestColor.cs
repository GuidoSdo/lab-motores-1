using UnityEngine;

public class TestColor : MonoBehaviour
{
    public Renderer outlineRenderer;

    [Header("Color")]
    public Color outlineColor = Color.red;

    [Header("Intensity")]
    [Range(0f, 10f)]
    public float intensity = 3f;
    [Range(0f, 10f)]
    public float fresnelStrength = 3f;

    MaterialPropertyBlock mpb;

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
