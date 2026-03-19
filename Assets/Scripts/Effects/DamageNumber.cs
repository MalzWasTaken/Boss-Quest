using UnityEngine;
using TMPro;
using System.Collections;

public class DamageNumber : MonoBehaviour
{
    public float floatSpeed = 80f;
    public float fadeSpeed = 1.5f;
    public float lifetime = 1.2f;

    private TMP_Text text;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(float damage, bool isCrit)
    {
        text.text = ((int) damage).ToString();
        text.color = isCrit ? Color.yellow : Color.white;
        if (isCrit) text.fontSize = 40f;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while(elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / lifetime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void SetupHeal(float amount)
    {
        text.text = $"+{Mathf.RoundToInt(amount)}";
        text.color = Color.green;
        StartCoroutine(Animate());
    }
}