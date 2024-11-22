using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Button saveButton;
    public Button loadButton;
    public int playerNumber;

    public GameObject[] remainingFruits;
    PlayerSP player;
    public bool[] activeFruitsTest;
    bool loadingScene;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        player = PlayerSP.instance;
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(player);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if (SceneManager.GetActiveScene().buildIndex != data.level)
        {
            SceneManager.LoadScene(data.level);
            loadingScene = true;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSP>();
            player.currentHealth = data.health;
            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];
            player.transform.position = position;

            player.fruitScore = data.fruitScore;
            player.fruits = data.fruits;
            player.maxFruits = data.maxFruits;
            SetActiveFruits(data.activeFruits);

            player.speed = data.speed;
            player.jumpForce = data.jumpForce;
        }

    }
    void SetActiveFruits(bool[] activeFruits)
    {
        activeFruitsTest = activeFruits;
        for (int i = 0; i < remainingFruits.Length; i++)
        {
            remainingFruits[i].SetActive(activeFruits[i]);
            remainingFruits[i].GetComponent<Collider2D>().enabled = true;
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        remainingFruits = GameObject.FindGameObjectsWithTag("Fruits");
        if (loadingScene == true)
        {
            LoadPlayer();
            loadingScene = false;
        }
    }
}
