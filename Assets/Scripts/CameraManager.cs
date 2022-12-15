using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager main;
    public enum State { ground, sky, transition, driver, storefront};
    Transform player;
    Transform skyTarget;
    Transform groundTarget;
    Transform driver;
    Transform storefront;

    Transform currentlyFollowing;
    GameObject trail;

    public State cameraState = State.ground;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        player = GameObject.Find("Player").transform;
        driver = GameObject.Find("Driver/DriverCameraPosition").transform;
        skyTarget = GameObject.Find("SkyCameraPosition").transform;
        groundTarget = GameObject.Find("GroundCameraPosition").transform;
        trail = GameObject.Find("Driver/Trail");
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraState == State.driver && currentlyFollowing)
        {
            trail.SetActive(false);
            player.position = Vector3.MoveTowards(player.position, currentlyFollowing.position, 5 * Time.deltaTime);
            //player.position = currentlyFollowing.position;
            float angle = Quaternion.Angle(player.rotation, currentlyFollowing.rotation);
            //player.rotation = currentlyFollowing.rotation;
            player.rotation = Quaternion.RotateTowards(player.rotation, currentlyFollowing.rotation, 10 * angle * Time.deltaTime);
        }
        else
        {
            trail.SetActive(true);
        }
    }

    public bool ChangeState (State state)
    {
        if (state == State.transition)
        {
            return false;
        }
        switch(state)
        {
            case State.transition:
                return false;
            case State.driver:
                if (cameraState == State.sky)
                {
                    //StartCoroutine(fastPan(player, driver, State.driver, 0.1f, 1, 3));
                    StartCoroutine(moveTo(player, driver, State.driver));
                }else
                {
                    StartCoroutine(panTo(player, driver, State.driver));
                }
                break;
            case State.ground:
                StartCoroutine(panTo(player, groundTarget, State.ground));
                break;
            case State.sky:
                if (cameraState == State.driver)
                {
                    //StartCoroutine(fastPan(player, skyTarget, State.sky, 3, 1, 0.1f));
                    StartCoroutine(moveTo(player, skyTarget, State.sky));
                }
                else
                {
                    StartCoroutine(panTo(player, skyTarget, State.sky));
                }
                
                break;
        }
        return true;
    }

    public IEnumerator moveTo(Transform from, Transform to, State state)
    {
        cameraState = State.transition;
        float transitionTime = 3f;
        float t = 0;
        while (t < 1)
        {
            player.transform.position = Vector3.Lerp(from.position, to.position, t);
            //Quaternion targetLookDown = Quaternion.AngleAxis(90, Vector3.right);

            player.transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, t);
            t = t + Time.deltaTime / transitionTime;
            yield return new WaitForEndOfFrame();
        }
        cameraState = state;
        currentlyFollowing = to;
    }

    public IEnumerator panTo (Transform from, Transform to, State state)
    {
        if (Vector3.Distance(from.position, to.position) < 50f)
        {
            StartCoroutine(moveTo(from, to, state));
            yield break;
        }
        
        cameraState = State.transition;
        float height = 100f;

        // zoomout
        Vector3 original = from.position;
        Quaternion originalRotation = from.rotation;
        Vector3 targetZoomOut = new Vector3(original.x, height, original.z);
        float transitionTime = 3f;
        float t = 0;
        Quaternion targetLookDown = Quaternion.AngleAxis(90, Vector3.right);
        while (t < 1)
        {
            float k = Mathf.SmoothStep(0, 1, t);

            player.transform.position = Vector3.Lerp(original, targetZoomOut, k);
            player.transform.rotation = Quaternion.Lerp(originalRotation, targetLookDown, k);
            t = t + Time.deltaTime / transitionTime;
            yield return new WaitForEndOfFrame();
        }

        // pan
        Vector3 targetZoomOutMove = new Vector3(to.position.x, height, to.position.z);
        t = 0;
        while (t < 1)
        {
            player.transform.position = Vector3.Lerp(targetZoomOut, targetZoomOutMove, Mathf.SmoothStep(0,1,t));
            t = t + Time.deltaTime / transitionTime;
            yield return new WaitForEndOfFrame();
        }

        // zoomin
        originalRotation = player.transform.rotation;

        t = 0;
        while (t < 1)
        {
            float k = Mathf.SmoothStep(0, 1, t);
            player.transform.rotation = Quaternion.Slerp(originalRotation, to.transform.rotation, k);
            player.transform.position = Vector3.Lerp(targetZoomOutMove, to.position, k);
            t = t + Time.deltaTime / transitionTime;
            yield return new WaitForEndOfFrame();
        }
        cameraState = state;
        currentlyFollowing = to;
    }

    public IEnumerator fastPan (Transform from, Transform to, State state, float timeIn, float hold, float timeOut)
    {
        cameraState = State.transition;
        float height = 100f;

        // zoomout
        Vector3 original = from.position;
        Quaternion originalRotation = from.rotation;
        Vector3 targetZoomOut = new Vector3(original.x, height, original.z);
        float t = 0;
        Quaternion targetLookDown = Quaternion.AngleAxis(90, Vector3.right);
        while (t < 1)
        {
            float k = Mathf.SmoothStep(0, 1, t);

            player.transform.position = Vector3.Lerp(original, targetZoomOut, k);
            player.transform.rotation = Quaternion.Lerp(originalRotation, targetLookDown, k);
            t = t + Time.deltaTime / timeIn;
            yield return new WaitForEndOfFrame();
        }

        // pan
        Vector3 targetZoomOutMove = new Vector3(to.position.x, height, to.position.z);
        t = 0;
        while (t < 1)
        {
            player.transform.position = Vector3.Lerp(targetZoomOut, targetZoomOutMove, Mathf.SmoothStep(0, 1, t));
            t = t + Time.deltaTime / hold;
            yield return new WaitForEndOfFrame();
        }

        // zoomin
        originalRotation = player.transform.rotation;

        t = 0;
        while (t < 1)
        {
            float k = Mathf.SmoothStep(0, 1, t);
            player.transform.rotation = Quaternion.Slerp(originalRotation, to.transform.rotation, k);
            player.transform.position = Vector3.Lerp(targetZoomOutMove, to.position, k);
            t = t + Time.deltaTime / timeOut;
            yield return new WaitForEndOfFrame();
        }
        cameraState = state;
        currentlyFollowing = to;
    }

}
