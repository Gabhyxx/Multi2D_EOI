using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PauseMenuMP : MonoBehaviour
{
    UnityEngine.UI.Button saveButton;
    UnityEngine.UI.Button loadButton;

    [Header("GameUI")]
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject firstSelectedOnPause;


    private void Awake()
    {
        inGamePanel = GameObject.Find("InGameUI");
        pausePanel = GameObject.Find("PausePanel");
        firstSelectedOnPause = GameObject.Find("MainMenuButton");

    }
    private void Start()
    {
        inGamePanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseAction();
        }
    }
    public void ToMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
    void PauseAction()
    {
        inGamePanel.SetActive(!inGamePanel.activeSelf);
        pausePanel.SetActive(!pausePanel.activeSelf);
        EventSystem.current.SetSelectedGameObject(firstSelectedOnPause);
    }
    public void TempQuitGame()
    {
        Debug.Log("Has salido del juego");
        Application.Quit();
    }
}
