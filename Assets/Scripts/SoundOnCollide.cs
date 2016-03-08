using UnityEngine;
using System.Collections;

public class SoundOnCollide : MonoBehaviour
{

    AudioSource source { get; set; }

    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        source.Play();
    }

    public void OnCollisionEnter(Collision collision)
    {
        source.Play();
    }

    public void OnCollisionStay(Collision collision)
    {

    }

    public void OnCollisionExit(Collision collision)
    {

    }
}
