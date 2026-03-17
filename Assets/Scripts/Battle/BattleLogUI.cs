using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleLogUI : MonoBehaviour
{
    public static BattleLogUI Instance;

    public GameObject logPanel;
    public TMP_Text logText;
    public float messageDisplayTime = 1.2f;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplaying = false;

    void Awake()
    {
        Instance = this;
        logPanel.SetActive(false);
    }

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isDisplaying)
            StartCoroutine(DisplayMessages());
    }

    IEnumerator DisplayMessages()
    {
        isDisplaying = true;
        logPanel.SetActive(true);

        while (messageQueue.Count > 0)
        {
            logText.text = messageQueue.Dequeue();
            yield return new WaitForSeconds(messageDisplayTime);
        }

        logPanel.SetActive(false);
        isDisplaying = false;
    }
}
