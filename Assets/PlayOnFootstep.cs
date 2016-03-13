using UnityEngine;
using System.Collections;

public class PlayOnFootstep : MonoBehaviour {

    AudioSource source; 
	// Use this for initialization
	void Start () {
        source = gameObject.GetComponent<AudioSource>(); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FootDown()
    {
        source.Play(); 
    }
}
