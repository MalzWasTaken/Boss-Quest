using UnityEngine;
using TMPro;

public class EnemyIndicator : MonoBehaviour
{
    public GameObject diamondIndicator;
    public GameObject nameCanvas;        // the whole world space canvas
    public TMP_Text nameLabel;

    private float bobSpeed = 2f;
    private float bobAmount = 0.15f;
    private Vector3 basePosition;
    private Camera battleCamera;

    void Start()
    {
        battleCamera = Camera.main;
        basePosition = diamondIndicator.transform.localPosition;
        diamondIndicator.SetActive(false);
        nameCanvas.SetActive(false);
    }

    void Update()
    {
        // Bob and spin diamond
        if (diamondIndicator.activeSelf)
        {
            float newY = basePosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            diamondIndicator.transform.localPosition = new Vector3(basePosition.x, newY, basePosition.z);
            diamondIndicator.transform.Rotate(0, 100f * Time.deltaTime, 0,Space.World);
        }

        // Always face camera
        if (nameCanvas.activeSelf && battleCamera != null)
        {
            nameCanvas.transform.LookAt(
                nameCanvas.transform.position + battleCamera.transform.rotation * Vector3.forward,
                battleCamera.transform.rotation * Vector3.up
            );
        }
    }

    public void ShowIndicator()
    {
        diamondIndicator.SetActive(true);
        nameCanvas.SetActive(true);
    }

    public void HideIndicator()
    {
        diamondIndicator.SetActive(false);
        nameCanvas.SetActive(false);
    }

    public void ShowNameOnly() => nameCanvas.SetActive(true);
    public void HideNameOnly() => nameCanvas.SetActive(false);

    public void SetName(string name)
    {
        if (nameLabel != null) nameLabel.text = name;
    }
}