using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

    Transform standardPos; 
	// Use this for initialization
	void Start () {

        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup()
    {
        standardPos = GameObject.Find("CamPos").transform ?? null;
        if (standardPos != null)
        {
            transform.position = standardPos.position;
            transform.forward = standardPos.forward;
            transform.SetParent(standardPos);
            
        }
    }
}
