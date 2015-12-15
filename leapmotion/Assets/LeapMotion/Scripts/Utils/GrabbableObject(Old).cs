/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class GrabbableObjectOld : MonoBehaviour {

  public bool useAxisAlignment = false;
  public Vector3 rightHandAxis;
  public Vector3 objectAxis;

  public bool rotateQuickly = true;
  public bool centerGrabbedObject = false;

  public Rigidbody breakableJoint;
  public float breakForce;
  public float breakTorque;

	private FixedJoint joint;
	private AudioSource sound;
	private AudioSource pickup;
	public GameObject board;

  protected bool grabbed_ = false;
  protected bool hovered_ = false;

	private Vector3 pos;
	private float xMin;
	private float xMax;
	private float yMin;
	private float yMax;
	private float xRatio;
	private float yRatio;
	private float octaves = 1f;
	private bool playedForBar;
	//length of a measure in seconds
	//assume 4/4 time for now
	private float barLength = 3f;
	private float elapsed;
	private Color pink;
	private Color green;
	public float barRatio;

	void Start() {
		AudioSource [] sources = this.gameObject.GetComponents <AudioSource> ();
//		sound = this.gameObject.GetComponent <AudioSource> ();
		sound = sources [0];
		pickup = sources [1];
		xMin = board.transform.position.x - board.transform.lossyScale.x * 5f;
		xMax = board.transform.position.x + board.transform.lossyScale.x * 5f;
		yMin = board.transform.position.y - board.transform.lossyScale.z * 5f;
		yMax = board.transform.position.y + board.transform.lossyScale.z * 5f;
		elapsed = 0f;
		//pink = new Color (1f, 0.7f, 0.7f);
		pink = new Color (1f, 1f, 1f);
		green = new Color (0.7f, 1f, 0.7f);
	}
  public bool IsHovered() {
    return hovered_;
  }

  public bool IsGrabbed() {
    return grabbed_;
  }

  public virtual void OnStartHover() {
    hovered_ = true;
  }

  public virtual void OnStopHover() {
    hovered_ = false;
  }

  public virtual void OnGrab() {
    grabbed_ = true;
    hovered_ = false;

    if (breakableJoint != null) {
      Joint breakJoint = breakableJoint.GetComponent<Joint>();
      if (breakJoint != null) {
        breakJoint.breakForce = breakForce;
        breakJoint.breakTorque = breakTorque;
      }
    }
		pickup.Play ();
  }

  public virtual void OnRelease() {
    grabbed_ = false;

    if (breakableJoint != null) {
      Joint breakJoint = breakableJoint.GetComponent<Joint>();
      if (breakJoint != null) {
        breakJoint.breakForce = Mathf.Infinity;
        breakJoint.breakTorque = Mathf.Infinity;
      }
    }
		if (transform.position.z > 0f) {
			GetComponent <Rigidbody> ().velocity = Vector3.forward * 10f;
		}
		pickup.Play ();
  }

  void OnCollisionEnter(Collision collision) {
		if (collision.collider.gameObject.tag == "board") {
			//pickup.Play ();
			FixedJoint tempJ = this.gameObject.AddComponent <FixedJoint> ();
			tempJ.breakForce = 500f;
			tempJ.connectedBody = collision.rigidbody;
			//board = collision.gameObject;

			pos = transform.position;
			xRatio = Mathf.Abs ((pos.x - xMin) / (board.transform.lossyScale.x * 10f));
//			Debug.Log (pos.x);
//			Debug.Log (xMin);
//			Debug.Log ("XRATIO: " + xRatio);
			yRatio = Mathf.Abs ((pos.y - yMin) / (board.transform.lossyScale.z * 10f));
//			Debug.Log ("YRATIO: " + yRatio);
			float aPitch = Mathf.Pow (2f, yRatio * octaves - 0.2f);
			sound.pitch = aPitch;
			float barRatio = elapsed / barLength;
			if(barRatio > xRatio) {
				playedForBar = true;
			}
			else {
				playedForBar = false;
			}
//			playedForBar = false;
			joint = this.gameObject.GetComponent <FixedJoint> ();
		}
	}

	void Update() {

		elapsed += Time.deltaTime;

		if (elapsed > barLength) {
			elapsed = 0f;
			playedForBar = false;
		}
		barRatio = elapsed / barLength;
		if (joint != null && !playedForBar) {
			//Debug.Log ("1stchecksatisfied");

			if(barRatio > xRatio && !sound.isPlaying) {
				sound.Play ();
				playedForBar = true;
			}

		}
		if (sound.isPlaying) {
			this.GetComponent<Renderer> ().material.color = green;
		} else {
			this.GetComponent<Renderer> ().material.color = pink;
		}
	}
}
