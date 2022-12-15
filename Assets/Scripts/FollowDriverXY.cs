using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDriverXY : MonoBehaviour
{
    GameObject driver;
    // Start is called before the first frame update
    void Start()
    {
        driver = GameObject.Find("Driver");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(driver.transform.position.x, transform.position.y, driver.transform.position.z);
    }
}
