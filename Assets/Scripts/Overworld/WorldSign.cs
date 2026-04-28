using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WorldSign : MonoBehaviour
{
    [TextArea(2, 6)]
    public string signText = "A sign.";
    public float interactRange = 3f;
    public TMP_Text promptText;
    public TMP_Text signDisplayText;

    private Transform player;
    private bool isReading = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (signDisplayText != null) signDisplayText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        bool inRange = Vector3.Distance(transform.position, player.position) <= interactRange;

        if (promptText != null)
            promptText.gameObject.SetActive(inRange && !isReading);

        if (inRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            isReading = !isReading;
            if (signDisplayText != null)
            {
                signDisplayText.text = signText;
                signDisplayText.gameObject.SetActive(isReading);
            }
        }

        if (!inRange && isReading)
        {
            isReading = false;
            if (signDisplayText != null)
                signDisplayText.gameObject.SetActive(false);
        }
    }
}
