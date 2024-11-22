using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    [SerializeField] private GameObject singleplayerPanel;

    GameObject player;
    LevelManager levelManager;

    public void Start()
    {
        singleplayerPanel.SetActive(false);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EnableSingleplayerPanel()
    {
        singleplayerPanel.SetActive(true);
    }

    public void DisableSingleplayerPanel()
    {
        singleplayerPanel?.SetActive(false);
    }
}
