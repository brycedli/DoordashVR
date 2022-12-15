using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Driver : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;
    GameObject driverParticles;
    Vector3 startPosition;
    public static Driver main;
    public Transform [] targets;
    Transform target;
    float speed = 2;
    int targetIncrement = 0;

    TextMeshPro textMesh;

    public enum State {stop, cruise };
    public State driveState = State.stop;

    float totalDist;
    // Start is called before the first frame update
    void Start()
    {

        textMesh = GameObject.Find("Driver/Timer").GetComponent<TextMeshPro>();
        driverParticles = GameObject.Find("Driver/ParticleHolder");
        driverParticles.SetActive(false);

        startPosition = transform.position;
        main = this;
        if (targets.Length == 0)
        {
            target = GameObject.Find("DriverTarget").transform;
            totalDist = 50f;
        }
        else
        {
            target = targets[targetIncrement];
            totalDist = Vector3.Distance(targets[0].position, targets[targets.Length - 1].position);

        }


        source.clip = clip;

    }

    public void startDrive ()
    {
        driveState = State.cruise;
        source.Play();
        driverParticles.SetActive(true);
    }

    public void stopDrive ()
    {
        driveState = State.stop;
        source.Stop();
        driverParticles.SetActive(false);
    }


    public void resetDriver ()
    {
        targetIncrement = 0;
        driveState = State.stop;
        source.Stop();
        driverParticles.SetActive(false);
        speed = 2;
        transform.position = startPosition;
        target = targets[0];
    }
    // Update is called once per frame
    void Update()
    {
        float dist2Final = Vector3.Distance(transform.position, targets[targets.Length - 1].position);
        int timeLeft = Mathf.FloorToInt(dist2Final / totalDist * 12);
        textMesh.text = $"{timeLeft} min";
        RaycastHit hit;
        //print(driveState);
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (driveState == State.stop)
            {
                startDrive();
            }
            else if (driveState == State.cruise)
            {
                stopDrive();
            }
        }
        
        if (Physics.Raycast(transform.position + Vector3.up * 5, Vector3.down, out hit) && driveState == State.cruise)
        {
            
            Vector3 targetPosition = new Vector3(target.position.x, hit.point.y, target.position.z);
            float dist = Vector3.Distance(transform.position, targetPosition);
            transform.position += transform.forward * speed * Time.deltaTime;

            //if (dist < 5)
            //{
            //    speed = dist;
            //}else
            //{
            //    speed = 2;
            //}

            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            //transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
            //Vector3 lookTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            Vector3 lookDir = targetPosition - transform.position;
            Quaternion q = Quaternion.LookRotation(lookDir);
            float angle = Quaternion.Angle(q, transform.rotation);
            if (angle > 1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, angle * Time.deltaTime);

            }
            Vector3 groundTarget = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, groundTarget, Time.deltaTime);
            //transform.LookAt(lookTarget);

            if (dist < 2f)
            {
                
                if (targetIncrement >= targets.Length-1)
                {
                    stopDrive();
                    speed = dist;
                    WalkthroughManager.main.endDrive();
                    return;
                }
                targetIncrement += 1;
                target = targets[targetIncrement];
            }
        }
        
    }
}
