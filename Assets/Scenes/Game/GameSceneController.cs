using UnityEngine;
using System.Collections;
using Photon;
using System;
using Assets.Marco_Polo;
using TouchScript.Gestures;
using TouchScript;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneController : PunBehaviour
{
    public List<Transform> SpawnPositions;
    public Button quitButton;
    public const float version = 0.31f;
    public const string gameName = "PUNTutorial";
    public List<GameObject> GirlPrefabList = new List<GameObject>(); 

    public float startingSpawnDist = 10; 
    public float maxSpawnDist = 40;
    public float maxSpawnDistIncrement = 10;     
    public float spawnY = 4;

    private int checksBeforeSearchExpand = 50; 

    public BoxCollider spawnChecker;

    GameObject myCharacter { get; set; }
    GirlController myCharacterController { get; set; }
    public string VersionAsString
    {
        get { return gameName + "_v" + version.ToString(); }
    }

    // Use this for initialization
    void Start()
    {
        quitButton.onClick.AddListener(delegate { OnButtonClicked(quitButton); });

        ConnectToServer();
    }

    int down = 0;
    // Update is called once per frame
    void Update()
    {
    }
    private void EnableCamera(GameObject monster)
    {
        var camera = Camera.main;
        camera.GetComponent<FollowCam>().Setup();
    }
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    private void InstantiateCharacter()
    {
        myCharacter = PhotonNetwork.Instantiate("CharacterPrefab", Vector3.zero, Quaternion.identity, 0);
        myCharacterController = myCharacter.GetComponent<GirlController>();
        myCharacterController.ToggleNameDisplay(false);



        //myCharacter.GetPhotonView().RPC("LoadModel", PhotonTargets.All, modelName); 
        var campfire = GameObject.Find("Campfire");

        EnableCamera(myCharacter);
        PlaceCharacterAtSpawn();

        Vector3 lookAt = new Vector3(campfire.transform.position.x, myCharacter.transform.position.y, campfire.transform.position.z);
        myCharacter.transform.LookAt(lookAt);
    }
    private void PlaceCharacterAtSpawn()
    {
        float spawnDistance = startingSpawnDist;
        int expandSearch = checksBeforeSearchExpand;  
        Vector3 position; 
        bool spawnOK = false;
        do
        {
            var pos = UnityEngine.Random.insideUnitCircle;
            pos *= spawnDistance;
            position = new Vector3(pos.x, spawnY, pos.y);
            spawnOK = Physics.CheckBox(new Vector3(pos.x, spawnY, pos.y), spawnChecker.size);
            expandSearch--; 

            if (expandSearch == 0 && spawnOK == false && spawnDistance < maxSpawnDist)
            {
                spawnDistance = Math.Min(spawnDistance + maxSpawnDistIncrement, maxSpawnDist);
                expandSearch = checksBeforeSearchExpand; 
            }
        }
        while (spawnOK == false);
        spawnChecker.transform.position = position;
        //var num = UnityEngine.Random.Range(0, SpawnPositions.Count - 1);
        position.y = myCharacter.GetComponent<CapsuleCollider>().height/2; 
        myCharacter.transform.position = position; //SpawnPositions[num].position;
        
    }
    public void OnButtonClicked(Button buttonClicked)
    {
        if (buttonClicked.name == quitButton.name)
        {
            iTween.Stop();
            iTween.Stop(gameObject);
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnDestroy()
    {

    }


    #region Photon
    private void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings(VersionAsString);
    }
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        InstantiateCharacter();
    }

    #endregion
}
