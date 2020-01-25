using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Camera))]
public class MixedInputInteraction : MonoBehaviour
{

    public LayerMask layerMask;

    void Awake()
    {
    }
    bool isTouching = false;
    Vector3 touch = Vector3.zero;
    void Update()
    {

        //if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        //    return;

        if (Input.touchCount > 0 || Input.GetMouseButton(0)) //if we are doing input
        {
            if (!isTouching) //and not previously touching
            {
                isTouching = true;
                //Debug.Log("Begin Touch");
                if (Input.touchCount > 0) //if we are doing input and not already touching
                {
                    //Debug.Log("Touchscreen Touch");
                    //touch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                    commitTouch(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
                }
                else if (Input.GetMouseButton(0))
                {
                    //Debug.Log("Mouse Touch");
                    commitTouch(Input.mousePosition);
                    //Debug.Log(Input.mousePosition);
                }
            }
        }
        else if(isTouching)
        {
            //Debug.Log("Stopped Touching");
            isTouching = false;
        }
        //Debug.Log(isTouching);
    }

    void commitTouch(Vector3 input)
    {

        RaycastHit hit;

        Ray ray = this.GetComponent<Camera>().ScreenPointToRay(input);
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

            //Debug.Log("Did Hit" + hit.transform.gameObject.name);

            GameObject hitObject = hit.transform.gameObject;
            //if (hitObject.GetComponent<LoadTotem>())
            //{
            //    hitObject.GetComponent<LoadTotem>().LoadObject();
            //}
            if ((hitObject.GetComponent<Button>()))
            {
                hitObject.GetComponent<Button>().onClick.Invoke();
            }
            //if ((hitObject.GetComponent<R3D_Deligate>()))
            //{
            //    hitObject.GetComponent<R3D_Deligate>().invokeSelected();
            //}
            //else
            //{
            //}


        }
    }
}
