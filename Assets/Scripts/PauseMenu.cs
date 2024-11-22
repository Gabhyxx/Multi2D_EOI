using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    UnityEngine.UI.Button saveButton;
    UnityEngine.UI.Button loadButton;

    [Header("GameUI")]
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject firstSelectedOnPause;


    private void Awake()
    {
        saveButton = GameObject.Find("SaveButton").GetComponent<UnityEngine.UI.Button>();
        loadButton = GameObject.Find("LoadButton").GetComponent<UnityEngine.UI.Button>();

        saveButton.onClick.AddListener(() => { GameManager.instance.SavePlayer(); });
        loadButton.onClick.AddListener(() => { GameManager.instance.LoadPlayer(); });

    }
    private void Start()
    {
        GameManager.instance.saveButton = saveButton;
        GameManager.instance.loadButton = loadButton;
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
        SceneManager.LoadScene(0);
    }

    public void LoadInteractable()
    {
        string path = Application.persistentDataPath + "/player.gbx";

        if (File.Exists(path) == false)
        {
            loadButton.interactable = false;
        }
        else
        {
            loadButton.interactable = true;
        }
    }
    void PauseAction()
    {
        inGamePanel.SetActive(!inGamePanel.activeSelf);
        pausePanel.SetActive(!pausePanel.activeSelf);
        EventSystem.current.SetSelectedGameObject(firstSelectedOnPause);

        if (pausePanel.activeSelf == true)
        {
            LoadInteractable();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
