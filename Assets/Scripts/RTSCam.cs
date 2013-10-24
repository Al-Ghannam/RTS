using UnityEngine;
using System.Collections;

public class RTSCam : MonoBehaviour
{

    public int maxHeight = 200;
    public int minHeight = 50;
    public int defaultHeight = 120;
    public int zoomSpeed = 4;
    public int rotateSpeed = 5;
    public int moveMargin = 5;
    public int moveSpeed = 3;

    private bool isRotating = false;

	// Use this for initialization
	void Start () {
        //apply height constraints then translate to default
        defaultHeight = Mathf.Clamp(defaultHeight, minHeight, maxHeight);
        transform.Translate(new Vector3(0, defaultHeight - transform.position.y, 0), Space.World);
	}
	
	// Update is called once per frame
	void Update () {
        ApplyZoom();
        ApplyRotation();
        ApplyMovement(); 
	}

    void ApplyZoom()
    {
        //when the user scrolls
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            //calculate new height
            int zoomTranslate = scrollInput > 0 ? -zoomSpeed : zoomSpeed;
            int newHeight = (int) transform.position.y - zoomTranslate;

            //apply new height clamped to min & max
            if (newHeight < minHeight && zoomTranslate < 0)
                transform.Translate(new Vector3(0, Mathf.Ceil(minHeight - transform.position.y), 0), Space.World);
            else if (newHeight > maxHeight && zoomTranslate > 0)
                transform.Translate(new Vector3(0, Mathf.Floor(maxHeight - transform.position.y), 0), Space.World);
            else
                transform.Translate(new Vector3(0, zoomTranslate, 0), Space.World);
        }
    }

    void ApplyRotation()
    {
        //set rotation when the middle mouse button is held
        if (Input.GetMouseButtonDown(2))
            isRotating = true;
        if (Input.GetMouseButtonUp(2))
            isRotating = false;

        if (isRotating)
        {
            float deltaMouseX = Input.GetAxis("Mouse X");
            if (deltaMouseX != 0)
            {
                //get the rotation point
                float xRot = transform.rotation.eulerAngles.x * Mathf.Deg2Rad;
                Vector3 forwardDistance = transform.forward * transform.position.y * Mathf.Tan(xRot); //set look distance (magnitude)
                Vector3 lookPoint = forwardDistance + transform.position;

                transform.RotateAround(lookPoint, new Vector3(0,1,0), deltaMouseX * 5);
            }
        }
    }

    void ApplyMovement()
    {
        if (isRotating)
            return;
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        
        if (mouseX < moveMargin)
        {
            Vector3 leftVec = Vector3.Cross(transform.forward, transform.up);
            leftVec.Normalize();
            leftVec *= moveSpeed;
            transform.Translate(leftVec, Space.World);
        }
        else if (mouseX > Screen.width - moveMargin)
        {
            Vector3 rightVec = -Vector3.Cross(transform.forward, transform.up);
            rightVec.Normalize();
            rightVec *= moveSpeed;
            transform.Translate(rightVec, Space.World);
        }
        else if (mouseY > Screen.height - moveMargin)
        {
            Vector3 upVec = new Vector3(transform.up.x, 0, transform.up.z);
            upVec.Normalize();
            upVec *= moveSpeed;
            transform.Translate(upVec, Space.World);
        }
        else if (mouseY < moveMargin)
        {
            Vector3 upVec = -(new Vector3(transform.up.x, 0, transform.up.z));
            upVec.Normalize();
            upVec *= moveSpeed;
            transform.Translate(upVec, Space.World);
        }
    }
}

