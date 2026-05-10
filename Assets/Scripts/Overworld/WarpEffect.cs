using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class WarpEffect : MonoBehaviour
{
    public static WarpEffect Instance;

    public Volume warpVolume;
    public float effectDuration = 1f;

    void Awake() { Instance = this; }

    public void TriggerWarp(System.Action onComplete)
    {
        Debug.Log($"[Warp] TriggerWarp called. warpVolume = {warpVolume}, gameObject active = {gameObject.activeInHierarchy}");

        StartCoroutine(WarpRoutine(onComplete));
    }

    IEnumerator WarpRoutine(System.Action onComplete)
    {
        if (warpVolume == null) { onComplete?.Invoke(); yield break; }

        float elapsed = 0f;
        while (elapsed < effectDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / effectDuration;
            if (warpVolume == null) yield break;
            warpVolume.weight = t;
            yield return null;
        }

        if (warpVolume != null) warpVolume.weight = 1f;
        onComplete?.Invoke();
    }
}