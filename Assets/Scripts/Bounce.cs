using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public Material bounceMaterial;
    public static Bounce main;
    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(bounce(LabelManager.main.currentlyLooking.gameObject.transform.position));
        //}
    }

    public void bounceHelper ()
    {
        StartCoroutine(bounce(LabelManager.main.currentlyLooking.gameObject.transform.position)); 
    }

    IEnumerator bounce (Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 30f);
        //print(hitColliders.Length);
        //GameObject mainBuilding = LabelManager.main.currentlyLooking.data.building;
        //if (mainBuilding)
        //{
        //    BounceInstance instance = mainBuilding.AddComponent<BounceInstance>();
        //    instance.StartCoroutine(instance.bounceInstant(0, 20));
        //    Renderer r = instance.gameObject.GetComponent<Renderer>();
        //    Material orgM = r.material;
        //    r.material = bounceMaterial;
            
        //    yield return new WaitForSeconds(0.7f);
        //    r.material = orgM;
        //}
        
        foreach (Collider c in hitColliders)
        {

            if (c.gameObject.CompareTag("Building"))
            {
                float distanceMag = (position - c.bounds.center).sqrMagnitude;
                BounceInstance instance = c.gameObject.AddComponent<BounceInstance>();
                instance.StartCoroutine(instance.bounceInstant(distanceMag/800, 10)); //Mathf.Clamp( 50-distanceMag/10, 0, 50)
            }
            
        }


        yield return null;
    }
}
