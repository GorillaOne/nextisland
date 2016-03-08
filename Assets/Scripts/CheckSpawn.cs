using UnityEngine;
using System.Collections;

public class CheckSpawn : MonoBehaviour {

    BoxCollider collider { get; set; }
	// Use this for initialization
	void Start () {
        collider = GetComponent<BoxCollider>(); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsSpaceEmpty()
    {
        return Physics.CheckBox(collider.transform.position, collider.size / 2); 
    }
}
