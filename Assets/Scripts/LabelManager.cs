using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LabelManager : MonoBehaviour
{
    public static LabelManager main;
    public GameObject labelUIPrefab;
    public Label currentlyLooking;
    Label currentlyLookingLastFrame;
    float timeLooking;
    public float timeHoldingLever;

    GameObject[] labelGOs;
    List<Label> labelUIs;

    public State state = State.Hovering;
    State lastFrameState;

    
    [SerializeField]
    public enum State
    {
        None,
        Hovering,
        Selected,
        Ordering
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        if (labelUIPrefab == null)
        {
            Debug.LogError("No label prefab selected");
        }

        labelGOs = GameObject.FindGameObjectsWithTag("Label");

        if (labelGOs.Length == 0)
        {
            Debug.LogWarning("No labels in scene");
        }

        labelUIs = new List<Label>();
        Transform canvas = GetComponent<Canvas>().transform;
        foreach (GameObject label in labelGOs)
        {
            labelUIs.Add(new Label(label, labelUIPrefab, canvas));
        }
        currentlyLooking = Label.ClosestLabel(labelUIs.ToArray());
        currentlyLookingLastFrame = currentlyLooking;
        StartCoroutine(currentlyLooking.popUp());
    }

    // Update is called once per frame
    void Update()
    {
        print(state);
        
        currentlyLooking = Label.ClosestLabel(labelUIs.ToArray());
        
        if (currentlyLooking != currentlyLookingLastFrame)
        {
            
            StartCoroutine(currentlyLooking.popUp());
            StartCoroutine(currentlyLookingLastFrame.popDown());
            

            if (lastFrameState == State.Selected &&  state == State.Hovering)
            {
                StartCoroutine(currentlyLookingLastFrame.endFocus());
            }
            if (lastFrameState == State.Ordering)
            {
                StartCoroutine(currentlyLookingLastFrame.endOrder());
                StartCoroutine(currentlyLookingLastFrame.endFocus());
                
            }
            /*if (currentlyLooking == null)
            {
                state = State.None;
            }*/
            timeLooking = Time.time;
        }

        var leftHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandedControllers);
        bool isHoldingTrigger = false;
        foreach (var device in leftHandedControllers)
        {
            float triggerValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue))
            {
                isHoldingTrigger = true;
                //Debug.Log($"Trigger button is pressed. Value is {triggerValue}");
                timeHoldingLever = triggerValue;
            }
            //Debug.Log(string.Format("Device name '{0}' has characteristics '{1}'", device.name, device.characteristics.ToString()));
        }
        
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            timeHoldingLever = Time.time;
        }
        */
        if ( isHoldingTrigger == true && timeHoldingLever > 0.95f && state == State.Selected)
        {
            print("Starting order");
            state = State.Ordering;
            StartCoroutine(currentlyLooking.startOrder());
        }


        if (Time.time - timeLooking > 1.5f && state == State.Hovering)
        {

            timeLooking = Time.time;
            if (currentlyLooking != null)
            {
                state = State.Selected;
                StartCoroutine(currentlyLooking.startFocus());
                
            }
        }

        currentlyLookingLastFrame = currentlyLooking;
        lastFrameState = state;
        foreach (Label label in labelUIs)
        {
            label.updateLabel();
        }

        
    }


    public class Label
    {
        public GameObject gameObject;
        GameObject childUILabel;
        GameObject childCard;
        GameObject orderCard;
        GameObject button;
        GameObject orderSubcard;

        public LabelData data;
        GameObject label;
        bool isFocused = false;
        bool showData;

        const float overallScale = 0.06f;

        public Label (GameObject gameObject, GameObject prefab, Transform canvas)
        {
            this.gameObject = gameObject;
            if (gameObject.GetComponent<LabelData>())
            {
                data = gameObject.GetComponent<LabelData>();
            }else
            {
                data = gameObject.AddComponent<LabelData>();
            }
            //label = Instantiate(prefab, canvas);
            label = Instantiate(prefab, gameObject.transform);
            childUILabel = label.transform.Find("Pin").gameObject;
            childCard = label.transform.Find("CardButtonUI").gameObject;
            orderCard = childCard.transform.Find("Mask/CardScan").gameObject;
            button = childCard.transform.Find("MainCard/Button").gameObject;
            orderSubcard = childCard.transform.Find("MainCard").gameObject;
            //orderCard.GetComponent<CanvasRenderer>().SetColor(new Color(1, 1, 1, 0));
            orderCard.GetComponent<CanvasRenderer>().SetAlpha(0);
            orderCard.SetActive(false);
            //print(orderCard.GetComponent<CanvasRenderer>().GetColor());
            childCard.GetComponent<RectTransform>().localScale = Vector3.zero;
        }

        public void updateLabel ()
        {
            //print(CameraManager.main.cameraState);
            if (CameraManager.main.cameraState != CameraManager.State.ground)
            {
                
                label.SetActive(false);
                return;
            }
            label.transform.LookAt(2 * label.transform.position - Camera.main.transform.position);
            label.transform.localScale = overallScale * Vector3.one;
            label.SetActive(true);
            childUILabel.SetActive(true);
            //label.transform.position = position;
            //childCard.SetActive(false);
            if (isFocused)
            {
                //if (LabelManager.main.state == LabelManager.State.Selected)
                //{
                //    childCard.SetActive(true);
                //}
                if (LabelManager.main.state == LabelManager.State.Hovering)
                {
                    float timeToPulse = 0.5f;
                    float s = (1 - Time.time / timeToPulse) - Mathf.Floor(1 - Time.time / timeToPulse);
                    childUILabel.GetComponent<RectTransform>().localScale =  Mathf.Lerp(1.8f, 2, s) * Vector3.one;
                }
                if (LabelManager.main.state == LabelManager.State.Selected)
                {
                    float k = LabelManager.main.timeHoldingLever;
                    if (k > 0.05f)
                    {
                        float xAxis = Mathf.Lerp(0, -90, k);
                        button.transform.localEulerAngles = new Vector3(xAxis, 0, 0);
                        button.transform.Find("Lower").GetComponent<CanvasRenderer>().SetColor(new Color(1 - k, 1 - k, 1 - k));
                    }
                    else
                    {
                        button.transform.localEulerAngles = new Vector3(0, 0, 0);
                        button.transform.Find("Lower").GetComponent<CanvasRenderer>().SetColor(new Color(1, 1, 1));
                    }

                }
            }
            else
            {
                childUILabel.GetComponent<CanvasRenderer>().SetAlpha(0.5f);
                childUILabel.GetComponent<RectTransform>().localScale = Vector3.one;
            }

        }

        public float distToCenter ()
        {
            Vector3 position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            Vector2 center = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
            Vector2 position2 = new Vector2(position.x, position.y);
            return Vector2.Distance(center, position);

        }

        public IEnumerator startOrder ()
        {

            float length = 0.2f;
            float t = 0;

            VideoManager.main.stopVideo();
            orderCard.SetActive(true);
            while (t < 1)
            {
                orderCard.GetComponent<CanvasRenderer>().SetAlpha(Mathf.SmoothStep(0, 1, t));
                orderSubcard.GetComponent<RectTransform>().localScale = Vector3.Lerp(Vector3.one, 0 * Vector3.one, t);
                t = t + Time.deltaTime / length;
                yield return new WaitForEndOfFrame();
            }
            orderSubcard.GetComponent<RectTransform>().localScale = Vector3.zero;
            orderCard.GetComponent<CanvasRenderer>().SetAlpha(1);
            yield return new WaitForSeconds(0.1f);
            Bounce.main.bounceHelper();
            yield return new WaitForSeconds(4.0f);

            WalkthroughManager.main.startDrive();
        }

        public IEnumerator endOrder ()
        {
            float length = 0.2f;
            float t = 0;
            while (t < 1)
            {
                orderCard.GetComponent<CanvasRenderer>().SetAlpha(Mathf.SmoothStep(1, 0, t));
                t = t + Time.deltaTime / length;
                yield return new WaitForEndOfFrame();
            }
            orderCard.SetActive(false);
            
        }

        public IEnumerator startFocus ()
        {
            VideoManager.main.resetVideo();
            orderSubcard.GetComponent<RectTransform>().localScale = Vector3.one;
            //LabelManager.main.state = LabelManager.State.Selected;
            float length = 0.2f;
            float t = 0;
            while (t < 1)
            {
                float k = Mathf.Sqrt(t);
                float endScale = 1.5f;
                
                childCard.GetComponent<RectTransform>().localScale =  Vector3.Lerp(0 * Vector3.one, endScale * Vector3.one, k);
                childUILabel.GetComponent<RectTransform>().localScale = Vector3.Lerp(2 * Vector3.one, Vector3.one, k);
                t = t + Time.deltaTime / length;
                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator endFocus ()
        {
            VideoManager.main.stopVideo();
            float length = 0.2f;
            float t = 0;
            while (t < 1)
            {
                float k = Mathf.Sqrt(t);
                float endScale = 0;
                childCard.GetComponent<RectTransform>().localScale = Vector3.Lerp(Vector3.one, endScale * Vector3.one, k);
                t = t + Time.deltaTime / length;
                yield return new WaitForEndOfFrame();
            }
            childCard.GetComponent<RectTransform>().localScale = Vector3.zero;
           
        }

        public IEnumerator popUp ()
        {
            isFocused = true;

            float length = 0.2f;
            float t = 0;
            //Vector3 startingScale = childUILabel.GetComponent<RectTransform>().localScale;

            while (t < 1)
            {
                float k = Mathf.SmoothStep(0.0f, 1.0f, t);
                childUILabel.GetComponent<RectTransform>().localScale = Vector3.Lerp(Vector3.one, 2 * Vector3.one, k);
                childUILabel.GetComponent<CanvasRenderer>().SetColor(new Color(1, 1, 1, Mathf.Lerp(0.5f, 1f, k)));
                //childCard.GetComponent<RectTransform>().localScale = Vector3.Lerp( 0 * Vector3.one, Vector3.one, k);
                t = t + Time.deltaTime / length;
                yield return new WaitForEndOfFrame();
            }




        }

        public IEnumerator popDown()
        {

            //LabelManager.main.state = LabelManager.State.None;
            if (LabelManager.main.state == LabelManager.State.Selected)
            {
                LabelManager.main.state = LabelManager.State.Hovering;
                
            }
            //Vector3 startingScale = childUILabel.GetComponent<RectTransform>().localScale;
            float length = 0.2f;
            float t = 0;
            while (t < 1)
            {
                float k = Mathf.SmoothStep(0.0f, 1.0f, t);
                childUILabel.GetComponent<RectTransform>().localScale = Vector3.Lerp(Vector3.one * 2, Vector3.one, Mathf.SmoothStep(0.0f, 1.0f, t));
                childUILabel.GetComponent<CanvasRenderer>().SetColor(new Color(1, 1, 1, Mathf.Lerp(1f, 0.5f, k)));
                //childCard.GetComponent<RectTransform>().localScale = Vector3.Lerp(Vector3.one, 0 * Vector3.one, k);
                t = t + Time.deltaTime / length;
                yield return new WaitForEndOfFrame();
            }
            label.GetComponent<RectTransform>().localScale = Vector3.one;
            childUILabel.GetComponent<CanvasRenderer>().SetAlpha(0.5f);
            //}
            isFocused = false;

        }

        public static Label ClosestLabel(Label[] targets)
        {
            Label tMin = null;
            float minDist = Mathf.Infinity;
            foreach (Label label in targets)
            {
                float dist = label.distToCenter();
                if (dist < minDist)
                {
                    tMin = label;
                    minDist = dist;
                }
            }
            return tMin;
        }

        
    }

    
}

