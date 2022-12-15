using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkthroughManager : MonoBehaviour
{
    public static WalkthroughManager main;
    
    // Start is called before the first frame update
    void Start()
    {
        main = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LabelManager.main.gameObject.SetActive(true);
            CameraManager.main.ChangeState(CameraManager.State.ground);
            Driver.main.resetDriver();
        }

        if (Driver.main.driveState == Driver.State.cruise && Input.GetKeyDown(KeyCode.Space))
        {
            if (CameraManager.main.cameraState == CameraManager.State.driver)
            {
                CameraManager.main.ChangeState(CameraManager.State.sky);
            }
            else if (CameraManager.main.cameraState == CameraManager.State.sky)
            {
                CameraManager.main.ChangeState(CameraManager.State.driver);
            }
        }
    }

    public void startDrive ()
    {
        StartCoroutine(order());
        
    }

    IEnumerator order ()
    {
        //LabelManager.main.gameObject.SetActive(false);
        CameraManager.main.ChangeState(CameraManager.State.driver);
        yield return new WaitForSeconds(10.0f);
        Driver.main.startDrive();
    }

    public void endDrive()
    {
        StartCoroutine(endDriveHelper());
    }

    IEnumerator endDriveHelper ()
    {
        yield return new WaitForSeconds(4.0f);
        CameraManager.main.ChangeState(CameraManager.State.ground);
        LabelManager.main.state = LabelManager.State.Hovering;
        //LabelManager.main.gameObject.SetActive(true);
        yield return new WaitForSeconds(20.0f);
        Driver.main.resetDriver();

    }
}


