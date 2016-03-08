using UnityEngine;
using System.Collections;

public class PointAtCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {        
        if (Camera.current != null)
        {
            transform.LookAt(Camera.current.transform);
            transform.Rotate(new Vector3(0, 180, 0));
        }
	}
}
