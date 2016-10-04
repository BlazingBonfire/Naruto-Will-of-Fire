using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class boxPickUp : MonoBehaviour {
	public Text boxPickUpText;
	bool isPickedUp;

	private Transform parent;
	private Rigidbody parentRb;
	private GameObject camPivot;

	void Start() {
		boxPickUpText.text = "Start";
		isPickedUp = false;

		camPivot = GameObject.Find("Main Camera Pivot");
		parent = transform.parent;
		parentRb = parent.gameObject.GetComponent<Rigidbody>();
	}

	void OnTriggerStay (Collider col){
		if (!isPickedUp) {
			if (!isPickedUp && col.tag == "Player") {
				boxPickUpText.text = "E: Pick Up";
				if (Input.GetKeyDown (KeyCode.E)) {
					parent.position = col.transform.position + Vector3.up * 2f;
					parent.parent = col.transform;
					parentRb.isKinematic = true;
					isPickedUp = true;
				} 
			} else {
				boxPickUpText.text = "not triggered";
			}
		} else {
			boxPickUpText.text = "isPickedUp";
			if (Input.GetKeyDown (KeyCode.E)) {
				parent.position = col.transform.position + camPivot.transform.TransformDirection(Vector3.forward) * 2f;
				parent.parent = null;
				parentRb.isKinematic = false;
				isPickedUp = false;
			}
		}
	}
}
