using UnityEngine;
using System.Collections;

public class camController : MonoBehaviour {
    private GameObject target;
    public float sensitivity = 4f;

    Vector3 offset;

    void Start(){
        target = GameObject.Find("Main Camera Pivot");
        offset = target.transform.position - transform.position;
    }

    void LateUpdate(){
        float horizontal = Input.GetAxis("Mouse X") * sensitivity;
        target.transform.Rotate(new Vector3(0, horizontal, 0), Space.World);
        //target.transform.rotation = transform.rotation * Quaternion.AngleAxis(horizontal, Vector3.up);

        float vertical = Input.GetAxis("Mouse Y") * -sensitivity;
        target.transform.Rotate(new Vector3(vertical, 0, 0), Space.Self);
        //target.transform.rotation = transform.rotation * Quaternion.AngleAxis(vertical, Vector3.right);

        offset = target.transform.position - transform.position;
        
        //scroll mouse wheel to zoom
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        //offset = offset.normalized * -mouseWheel * 5;

        transform.position += offset.normalized * mouseWheel * 100 * Time.deltaTime; //zooming
        
    }
}
