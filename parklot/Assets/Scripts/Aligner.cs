using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aligner : MonoBehaviour
{
    private int cycle = 0;
    public float connectionDistance = 2.0f;
    private GameObject[] gameObjects;
    private bool isConnected;
    private GameObject[] childs = new GameObject[2];
    private Dictionary<GameObject, List<Transform>> childsLocs;
    
    void Start()
    {
        childsLocs = new Dictionary<GameObject, List<Transform>>();
        gameObjects = GameObject.FindGameObjectsWithTag("obj");
        childs[0] = this.gameObject.transform.GetChild(0).gameObject;
        childs[1] = this.gameObject.transform.GetChild(1).gameObject;
        foreach (var obj in gameObjects)
        {
            GameObject a = obj.transform.GetChild(0).gameObject;
            GameObject b = obj.transform.GetChild(1).gameObject;
            childsLocs.Add(obj, new List<Transform>() { a.transform, b.transform });
        }

    }

    void Update()
    {
        if (isConnected)
            Destroy(this.GetComponent<Aligner>());
        Align(GetNearestPointToAttach());
    }
    void GetNearestNeighbour()
    {
        Collider[] hitCollidersA = Physics.OverlapSphere(childs[0].transform.position, connectionDistance);
        Collider[] hittCollidersB = Physics.OverlapSphere(childs[1].transform.position, connectionDistance);
    }
    PointPair GetNearestPointToAttach()
    {
        PointPair finalPoint = new PointPair();
        finalPoint.nearest = float.MaxValue;
        GameObject a = childs[0];
        GameObject b = childs[1];
        foreach (KeyValuePair<GameObject, List<Transform>> pair in childsLocs)
        {
            float currentAA = Vector3.Distance(a.transform.position, pair.Value[0].position);
            float currentBB = Vector3.Distance(b.transform.position, pair.Value[1].position);
            float currentAB = Vector3.Distance(a.transform.position, pair.Value[1].position);
            float currentBA = Vector3.Distance(b.transform.position, pair.Value[0].position);

            if (currentAA < currentBB && currentAA < currentBA && currentAA < currentAB)
            {
                if(currentAA<finalPoint.nearest)
                {
                finalPoint.firtPoint = a;
                finalPoint.secondPoint = pair.Value[0].gameObject;
                finalPoint.rotate = true;
                finalPoint.nearest = currentAA;
                }
            }
            else if (currentBB < currentAA && currentBB < currentAB && currentBB < currentBA)
            {
                if (currentBB < finalPoint.nearest)
                {
                    finalPoint.firtPoint = b;
                    finalPoint.secondPoint = pair.Value[1].gameObject;
                    finalPoint.rotate = true;
                    finalPoint.nearest = currentBB;
                }
            }
            else if (currentAB < currentAA && currentAB < currentBA && currentAB < currentBB)
            {
                if(currentAB<finalPoint.nearest)
                {
                    finalPoint.firtPoint = a;
                    finalPoint.secondPoint = pair.Value[1].gameObject;
                    finalPoint.rotate = false;
                    finalPoint.nearest = currentAB;
                }
                
            }
            else if (currentBA < currentAA && currentBA < currentAB && currentBA < currentBB)
            {
                if(currentBA<finalPoint.nearest)
                {
                    finalPoint.firtPoint = b;
                    finalPoint.secondPoint = pair.Value[0].gameObject;
                    finalPoint.rotate = false;
                    finalPoint.nearest = currentBA;

                }
                
            }

        }
        return finalPoint;
    }
    void Align(PointPair point)
    {
        GameObject firstPoint = point.firtPoint;
        GameObject secondPoint = point.secondPoint;
        bool rotate = point.rotate;

        Vector3 diff = secondPoint.transform.position - firstPoint.transform.position;
        firstPoint.transform.parent.transform.Translate(diff, Space.World);
        print(firstPoint.name + "" + secondPoint.name);

        //rotating will be occured at the next frame, then deattach the script.
        if (cycle == 1)
        {
            if (rotate)
                firstPoint.transform.parent.gameObject.transform.Rotate(Vector3.up, 180);
            this.isConnected = true;
            cycle = 0;
        }
        cycle++;
    }
    public struct PointPair
    {
        public bool rotate;
        public GameObject firtPoint;
        public GameObject secondPoint;
        public float nearest;

    };
}

