using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance;

    public GameObject damageNumberPrefab;
    public Canvas canvas;

    void Awake()
    {
        Instance = this;
    }

    public void Spawn(float damage, Vector3 worldPosition, bool isCrit = false)
    {
        Debug.Log($"Spawning Damage Number: {damage} at {worldPosition}");
        //converting world position to screen position
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        //converting screen position to canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            null,
            out Vector2 canvasPos
        );

        GameObject obj = Instantiate(damageNumberPrefab, canvas.transform);
        obj.GetComponent<RectTransform>().localPosition = canvasPos;
        obj.GetComponent<DamageNumber>().Setup(damage, isCrit);
    }

    public void SpawnHeal(float amount, Vector3 worldPosition)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            null,
            out Vector2 canvasPos
        );

        GameObject obj = Instantiate(damageNumberPrefab, canvas.transform);
        obj.GetComponent<RectTransform>().localPosition = canvasPos;
        obj.GetComponent<DamageNumber>().SetupHeal(amount);
    }
}