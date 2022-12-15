using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceInstance : MonoBehaviour
{
    public IEnumerator bounceInstant (float delay, float mag)
    {
        yield return new WaitForSeconds(delay);
        Vector3 originalPosition = transform.position;
        float length = 0.7f;
        float t = 0;
        while (t < 1)
        {
            t = t + Time.deltaTime / length;
            float h = -4 * (t - 0.5f) * (t - 0.5f) + 1;
            transform.position = originalPosition + Vector3.up * h * mag;
            yield return new WaitForEndOfFrame();
        }
        transform.position = originalPosition;
    }
}
