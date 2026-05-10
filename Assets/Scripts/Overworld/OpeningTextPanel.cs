using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class OpeningTextPanel : MonoBehaviour
{
    public GameObject panelRoot;
    public TMP_Text bodyText;
    public Button continueButton;

    [TextArea(3, 8)]
    public string message =
        "The realm has fallen to Wrecktrix, a demon of unfathomable power.\n\n" +
        "Slay the demons that roam these lands to grow stronger, then face Wrecktrix in his arena to end his reign.";

    public float typeSpeed = 0.03f; // seconds per character

    bool isTyping = false;
    bool typingComplete = false;

    void Start()
    {
         Debug.Log($"[OpeningPanel] Start called. hasShownIntro={BattleData.hasShownIntro}");
        if (BattleData.hasShownIntro)
        {
            Debug.Log("[OpeningPanel] Already shown, bailing");

            panelRoot.SetActive(false);
            return;
        }

        Debug.Log("[OpeningPanel] Setting suppress flag to TRUE");

        panelRoot.SetActive(true);
        Time.timeScale = 0f;

        // Take cursor control
        CameraFollow.SuppressCursorLock = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        continueButton.gameObject.SetActive(false);
        continueButton.onClick.AddListener(Dismiss);

        StartCoroutine(TypeText());
    }

    void Update()
{
    if (isTyping && (Keyboard.current.spaceKey.wasPressedThisFrame 
                     || Mouse.current.leftButton.wasPressedThisFrame))
    {
        StopAllCoroutines();
        CompleteTyping();
    }
}
    IEnumerator TypeText()
    {
        isTyping = true;
        bodyText.text = "";

        foreach (char c in message)
        {
            bodyText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        CompleteTyping();
    }

    void CompleteTyping()
    {
        bodyText.text = message;
        isTyping = false;
        typingComplete = true;
        continueButton.gameObject.SetActive(true);
        continueButton.Select(); // gives it keyboard focus, helps EventSystem register clicks
    }

    void Dismiss()
    {
        Debug.Log("[OpeningPanel] Dismiss called");
        BattleData.hasShownIntro = true;
        Time.timeScale = 1f;

        CameraFollow.SuppressCursorLock = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        panelRoot.SetActive(false);
    }
}