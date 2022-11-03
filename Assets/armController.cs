using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armController : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private Vector3[] armPositions;
    private GameObject Arm;

    // Start is called before the first frame update
    void Start()
    {
        //arm = Instantiate(armPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        //lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.GetPositions(armPositions);
        //Debug.Log(armPositions);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject oldLine = gameObject;
        GameObject newLine = Instantiate(oldLine);
        LineRenderer oldLineComponent = oldLine.GetComponent<LineRenderer>();

        //Get old Position Length
        Vector3[] newPos = new Vector3[oldLineComponent.positionCount];
        //Get old Positions
        oldLineComponent.GetPositions(newPos);

        Debug.Log(newPos.Length);
        Debug.Log("test ");

        //Copy Old postion to the new LineRenderer
        newLine.GetComponent<LineRenderer>().SetPositions(newPos);


        Vector3 endPos = newLine.GetComponent<LineRenderer>().GetPosition(newPos.Length - 1);
        Debug.Log(endPos);
    }
}
