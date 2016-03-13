using UnityEngine;
using System.Collections;
using System;
using Photon;
using UnityEngine.UI;
using Assets.Marco_Polo;
using System.Collections.Generic;

public class GirlController : PunBehaviour
{
    
    public bool InputControlEnabled { get; set; }
    float forwardSpeed;
    Vector3 correctPosition;
    Quaternion correctRotation;
    Rigidbody rb;
    TapCharacterControl tapController { get; set; }
    public Animator animator {get; set; }
    public List<GameObject> AvailableModels;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tapController = GetComponent<TapCharacterControl>() ?? null;
        CheckForRequiredComponents();
        if (photonView.isMine)
        {
            ToggleNameDisplay(false);
            tapController.InputEnabled = true;
            ChooseModel(); 
        }        
    }

    private void ChooseModel()
    {
        var index = UnityEngine.Random.Range(0, AvailableModels.Count);
        var modelName = AvailableModels[index].name;
        LoadModel(modelName);
        PhotonNetwork.RPC(photonView, "LoadModel", PhotonTargets.OthersBuffered, false, modelName);        
    }

    [PunRPC]
    private void LoadModel(string modelName)
    {       
        var go = AvailableModels.Find(a => a.name == modelName); 
        var model = GameObject.Instantiate(go);
        model.transform.SetParent(transform, false);
        model.transform.localPosition = Vector3.zero;
        model.transform.localEulerAngles = Vector3.zero;
        animator = model.GetComponent<Animator>(); 
    }

    private void CheckForRequiredComponents()
    {
        if (photonView == null) throw new Exception("This controller requires a photon view attached to the same game components.");
        if (tapController == null) throw new Exception("This controller requires a TapCharacterControl attached to the same game components.");
    }
    // Update is called once per frame
    void Update()
    {
        TrackMovement();
        NetworkMovementSmoothing();
    }

    void OnDestroy()
    {
        var sceneController = GameObject.Find("SceneController") ?? null;
        if (sceneController != null)
        {
            var cont = sceneController.GetComponent<GameSceneController>();
            cont.GirlPrefabList.Remove(this.gameObject);
        }
    }
    private void TrackMovement()
    {
        if (photonView.isMine)
        {
            forwardSpeed = rb.velocity.magnitude / TapCharacterControl.MaxMovementSpeed;
            if (animator != null)
            {
                animator.SetFloat("Forward", forwardSpeed);
            }
            
        }
    }
    public void ToggleNameDisplay(bool on)
    {
        var canvasGameObject = transform.FindChild("NameCanvas");
        canvasGameObject.gameObject.SetActive(false);
    }
    private void SetName(string name)
    {
        var canvas = GetComponentInChildren<Canvas>();
        var text = canvas.GetComponentInChildren<Text>();
        text.text = name;
    }
    private void NetworkMovementSmoothing()
    {
        if (!photonView.isMine && correctPosition != Vector3.zero && correctRotation != null)
        {
            transform.position = Vector3.Lerp(transform.position, correctPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctRotation, Time.deltaTime * 5);
        }
    }
    #region Photon
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        SetName(info.sender.name);
        var sceneController = GameObject.Find("SceneController");
        var cont = sceneController.GetComponent<GameSceneController>();
        cont.GirlPrefabList.Add(this.gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player, send the data. 
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(forwardSpeed);
        }
        else
        {
            //Network player, receieve the data. 
            correctPosition = (Vector3)stream.ReceiveNext();
            correctRotation = (Quaternion)stream.ReceiveNext();
            animator.SetFloat("Forward", (float)stream.ReceiveNext());
        }
    }


    #endregion
}
