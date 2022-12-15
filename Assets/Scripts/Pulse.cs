using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    float initialScale;
    float desiredScale;
    CanvasRenderer cRenderer;
    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale.x;
        desiredScale = initialScale * 1.2f;
        cRenderer = GetComponent<CanvasRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float timeToPulse = 0.5f;
        float s = (1 + Time.time / timeToPulse) - Mathf.Floor(1 + Time.time / timeToPulse);
        transform.localScale = Mathf.Lerp(initialScale, desiredScale, s) * Vector3.one;
        float k = (1 - Time.time / timeToPulse) - Mathf.Floor(1 - Time.time / timeToPulse);
        cRenderer.SetAlpha(k);
    }
}
