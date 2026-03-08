using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleLog : MonoBehaviour
{
    public static BattleLog Instance;

    public Transform content;
    public GameObject logLinePrefab;
    public ScrollRect scrollRect;

    void Awake()
    {
        Instance = this;
    }

    public void AddMessage(string message)
    {
        GameObject line = Instantiate(logLinePrefab, content);
        line.GetComponent<TMP_Text>().text = message;

        // Force scroll to bottom
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
