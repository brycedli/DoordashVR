using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPlacer : MonoBehaviour
{
    public GameObject building;
    const int n = 80;
    public GameObject[] buildings = new GameObject[n];

    // Start is called before the first frame update
    void Start()
    {
        if (!building)
        {
            Debug.LogError("Missing gameobject");
        }

        
        for (int i = 0; i < n; i++ )
        {
            float distanceFromCenter = Random.Range(4, 14);
            float theta = Random.Range(0, 2 * Mathf.PI);
            //Vector2 

            //Vector2 pos2D = Random.insideUnitCircle * 10;
            //float height = Random.Range(0.5f, 2.0f);
            float height = Random.Range(0, 0.5f) + distanceFromCenter/5;
            Vector3 pos3D = new Vector3(distanceFromCenter*Mathf.Sin(theta), height/2, distanceFromCenter*Mathf.Cos(theta));
            Vector3 scale = new Vector3(1, height, 1);
            GameObject buildingInstance = Instantiate(building);
            buildingInstance.transform.localScale = scale;
            buildingInstance.transform.position = pos3D;
            if (Random.Range(0, 4) == 0)
            {
                buildingInstance.GetComponent<Renderer>().material.color = Color.red;
            }
            buildings[i] = buildingInstance;
        }
    }

    // Update is called once per frame
    
}
