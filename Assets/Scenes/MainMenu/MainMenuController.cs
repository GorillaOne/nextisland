using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Assets.Marco_Polo;

public class MainMenuController : MonoBehaviour
{

    public InputField NameField;
    public Button JoinButton;
    public Button QuitButton; 

    // Use this for initialization
    void Start()
    {
        JoinButton.onClick.AddListener(delegate { OnButtonClicked(JoinButton); });
        QuitButton.onClick.AddListener(delegate { OnButtonClicked(QuitButton); });
    }

    // Update is called once per frame
    void Update()
    {
    }

    bool loadLevel = false; 
    public void OnButtonClicked(Button buttonClicked)
    {
        if(buttonClicked.name == JoinButton.name)
        {
            if(!String.IsNullOrEmpty(NameField.text))
            {
                PhotonNetwork.player.name = NameField.text;
                SceneManager.LoadScene("GameScene");
            }
        }
        else if (buttonClicked.name == QuitButton.name)
        {            
            Application.Quit(); 
        }
        else
        {
            //Case not handled. 
        }

    }


}
