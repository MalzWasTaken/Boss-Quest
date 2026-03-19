using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    public Color flashColor = Color.orange;
    public float flashDuration = 0.15f;
    public int flashCount = 2;

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.15f;

    [Header("Heal Settings")]
    public Color healColor = Color.green;

    private Renderer objectRenderer;
    private Color originalColor;
    private Vector3 originalPosition;

    public GameObject deathParticlesPrefab;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
        originalPosition = transform.localPosition;
    }

    public void PlayHitEffect()
    {
        StartCoroutine(Flash());
        StartCoroutine(Shake());
    }

    IEnumerator Flash()
    {
        for (int i = 0; i < flashCount; i++)
        {
            objectRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            objectRenderer.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = Random.Range(-shakeMagnitude, shakeMagnitude);

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public void PlayDeathEffect()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        // Play death animation first
        GetComponent<CombatantAnimator>()?.PlayDeathAnimation();
        
        // Wait a moment
        yield return new WaitForSeconds(1f);

        // Spawn particles at enemy position
        if (deathParticlesPrefab != null)
            Instantiate(deathParticlesPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        // Wait for particles to start then hide the model
        yield return new WaitForSeconds(0.2f);
        
        // Hide the model but keep the GameObject alive for battle logic
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
    }

    public void PlayHealEffect()
    {
        StartCoroutine(HealFlash());
    }

    IEnumerator HealFlash()
    {
        for (int i = 0; i < 2; i++)
        {
            float elapsed = 0f;
            float duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                objectRenderer.material.color = Color.Lerp(originalColor, healColor, elapsed / duration);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                objectRenderer.material.color = Color.Lerp(healColor, originalColor, elapsed / duration);
                yield return null;
            }
        }

        objectRenderer.material.color = originalColor;
    }

    // public void PlayDeathEffect()
    // {
    //     StartCoroutine(DeathCrush());
    // }

    IEnumerator DeathCrush()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        Vector3 originalScale = transform.localScale;
        Vector3 crushedScale = new Vector3(originalScale.x * 1.5f, 0f, originalScale.z * 1.5f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, crushedScale, elapsed / duration);
            yield return null;
        }

        transform.localScale = crushedScale;
        gameObject.SetActive(false);
    }
}