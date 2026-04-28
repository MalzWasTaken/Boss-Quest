using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    Vector3 lastPos;
    
    void Start()
    {
        lastPos = transform.position;
        Debug.Log($"[PositionTracker] Start position: {transform.position}");
    }
    
    void LateUpdate()
    {
        if (Vector3.Distance(lastPos, transform.position) > 0.5f)
        {
            Debug.Log($"[PositionTracker] Position jumped from {lastPos} to {transform.position}");
            lastPos = transform.position;
        }
    }
}