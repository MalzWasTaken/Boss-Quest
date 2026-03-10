
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class WarpEffect : MonoBehaviour
{
    public static WarpEffect Instance;

    public Material warpMaterial;
    public float effectDuration = 1f;

    private float warpAmount = 0f;
    private float blurAmount = 0f;
    private float chromaAmount = 0f;
    private bool isWarping = false;

    void Awake()
    {
        Instance = this;
    }

    public void TriggerWarp(System.Action onComplete)
    {
        StartCoroutine(WarpRoutine(onComplete));
    }

    IEnumerator WarpRoutine(System.Action onComplete)
    {
        isWarping = true;
        float elapsed = 0f;


        // Ramp up warp
        while (elapsed < effectDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / effectDuration;
            float eased = Mathf.Pow(t, 2f);

            warpAmount = Mathf.Lerp(0f, 10f, eased);
            blurAmount = Mathf.Lerp(0f, 0.3f, eased);
            chromaAmount = Mathf.Lerp(0f, 0.05f, eased);

            warpMaterial.SetFloat("_WarpAmount", warpAmount);
            warpMaterial.SetFloat("_BlurAmount", blurAmount);
            warpMaterial.SetFloat("_ChromaAmount", chromaAmount);

            yield return null;
        }

        onComplete?.Invoke();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (isWarping && warpMaterial != null)
            Graphics.Blit(src, dest, warpMaterial);
        else
            Graphics.Blit(src, dest);
    }
}