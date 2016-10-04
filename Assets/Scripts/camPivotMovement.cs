using UnityEngine;
using System.Collections;

public class camPivotMovement : MonoBehaviour {
    private GameObject target;
    // Use this for initialization
    public Vector3 offset;

    void Start () {
        target = GameObject.Find("Player");
        offset = target.transform.position - transform.position;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = target.transform.position + offset;
	}
}
