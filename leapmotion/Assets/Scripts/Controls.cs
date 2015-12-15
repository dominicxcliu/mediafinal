using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

	public GameObject syncCube;
	public GameObject playHead;
	private GrabbableObject go;

	// Use this for initialization
	void Start () {
		go = syncCube.GetComponent <GrabbableObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel (Application.loadedLevel);
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
		playHead.transform.position = new Vector3 ((-3.972f + 7.950212f * go.barRatio), 0.51f, 2.52f);
	}
}
