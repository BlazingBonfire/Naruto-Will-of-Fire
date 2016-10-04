using UnityEngine;
using System.Collections;

public class playerMovement : MonoBehaviour {
    private float moveSpeed;
	public float origMoveSpeed;

	public float jumpForce;
    private float jumpStartX;
    private float jumpStartZ;

	private float dashSpeed = 30f;
	private float dashDuration = .1f;
	private bool dashing = false;
	private float dashStartTime;
	private Vector3 dashTarget;

	private float doubleTapCooldown = .25f;
	private string lastButtonPressed = "";
	private float lastButtonTime;

    public Collider collid;
    public Rigidbody rb;
    public GameObject player;
    public GameObject camPivot;
    public Vector3 moveDirection;

    RaycastHit groundHit;
    private GameObject groundObj;

    void Start() {
		origMoveSpeed = 2.5f;
		jumpForce = 20f;
        collid = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        camPivot = GameObject.Find("Main Camera Pivot");
    }

	void Move(Vector3 direction){
		transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
		rb.velocity = new Vector3((direction * moveSpeed * Time.deltaTime).x * 100, rb.velocity.y, (direction * moveSpeed * Time.deltaTime).z * 100);
	}
	void Dash(Vector3 normalizedDirection){
		dashStartTime = 0;
		dashTarget = normalizedDirection;
		dashing = true;
  	}
	void Idle(){
		rb.velocity = Vector3.zero;
	}  

	bool isDoubleTapped(string button){
		float deltaTime = Time.time - lastButtonTime;
		if (lastButtonPressed == button && deltaTime <= doubleTapCooldown) {
			lastButtonPressed = "";
			lastButtonTime = Time.time;
			return true;
		}
		lastButtonPressed = button;
		lastButtonTime = Time.time;
		return false;
	} 

	void Update() {
		#region dash
		//dash inputs
		if (Input.GetButtonDown("Left")) {
			if(isDoubleTapped("Left")){
				Dash((Vector3.Scale(camPivot.transform.TransformDirection(Vector3.left), new Vector3(1, 0, 1))).normalized);
			}
		}
		if (Input.GetButtonDown("Right")) {
			if(isDoubleTapped("Right")){
				Dash((Vector3.Scale(camPivot.transform.TransformDirection(Vector3.right), new Vector3(1, 0, 1))).normalized);
			}
		}
		if (Input.GetButtonDown("Up")) {
			if(isDoubleTapped("Up")){
				Dash((Vector3.Scale(camPivot.transform.TransformDirection(Vector3.forward), new Vector3(1, 0, 1))).normalized);
			}
		}
		if (Input.GetButtonDown("Down")) {
			if(isDoubleTapped("Down")){
				Dash((Vector3.Scale(camPivot.transform.TransformDirection(Vector3.back), new Vector3(1, 0, 1))).normalized);
			}
		}
		//dash movement
		if(dashing) {
			dashStartTime += Time.deltaTime;
			Move(dashTarget);
			if (dashStartTime >= dashDuration){
				dashing = false;
				rb.velocity = Vector3.zero;
			}
		}
        #endregion
	}
    void FixedUpdate() {
		#region Get moveDirection
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 camPivotForward = (Vector3.Scale(camPivot.transform.TransformDirection(Vector3.forward), new Vector3(1, 0, 1))).normalized;
        Vector3 camPivotRight = new Vector3(camPivotForward.z, 0, -camPivotForward.x);
        moveDirection = (ver * camPivotForward + hor * camPivotRight).normalized;
		#endregion
		#region Idle (friction)
		if (IsGrounded() && moveDirection == Vector3.zero && Vector3.Angle(groundHit.normal, Vector3.up) < 60 && !Input.GetButtonDown("Jump")) {
			rb.constraints = RigidbodyConstraints.FreezeAll;
		} else {
			rb.constraints = RigidbodyConstraints.None;
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}
        #endregion
        #region Set Movement Speed
        if (IsSprinting()) {
            moveSpeed = origMoveSpeed * 1.7f;
        } else if (IsWalking()) {
			moveSpeed = origMoveSpeed * .3f;
		} else if(dashing) {
			moveSpeed = dashSpeed;
        } else {
            moveSpeed = origMoveSpeed;
        }
        #endregion
        #region Moving
        if (moveDirection != Vector3.zero && IsMovable() && IsGrounded()){
			Move(moveDirection);
		}

		if (moveDirection == Vector3.zero && IsMovable() && IsGrounded()) {
            Idle();
        }
        #endregion
        #region Smooth Moving  //Used for when jumping
        if (IsMovable() && !IsGrounded()) {
            rb.AddForce((moveDirection * moveSpeed * Time.deltaTime)* 5000);
        }
        #endregion 
        #region Jump
        if (Input.GetButtonDown("Jump") && IsGrounded()){
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            //rb.AddForce(0,jumpForce*160000*Time.deltaTime,0);
        }
        #endregion
		#region Falling
		if (IsFalling()) {
			rb.AddForce(new Vector3(0, -500, 0)); //additional gravity
		}
            		#endregion 

    }
    #region playerStates
    bool IsGrounded() {
		return Physics.SphereCast(transform.position, collid.bounds.size.x / 2 - .1f, -Vector3.up, out groundHit, collid.bounds.size.y / 2 + .01f);
    }
    bool IsFalling() {
        if (!IsGrounded()) {
            return true;
        }
        return false;
    }
    bool IsMovable() {
		if (!dashing) {
			if (IsGrounded () || IsFalling ()) {
				return true;
			}
		}
        return false;
    }
    bool IsIdle() {
        if (rb.velocity == Vector3.zero && moveDirection == Vector3.zero)
            return true;
        return false;
    }
    bool IsSprinting() {
        if (Input.GetButton("Sprint")) {
            return true;
        }
        return false;
    }
    bool IsWalking() {
        if (Input.GetButton("Walk")) {
            return true;
        }
        return false;
    }
    #endregion
}
