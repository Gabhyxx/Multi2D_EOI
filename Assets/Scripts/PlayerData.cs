using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    
    public bool[] activeFruits;

    public int level;
    public float health;
    public float[] position;
    public int fruitScore;
    public int fruits;
    public int maxFruits;

    public float speed;
    public float jumpForce;

    public PlayerData(PlayerSP player) 
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSP>();
        level = SceneManager.GetActiveScene().buildIndex;
        health = player.currentHealth;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        GetActiveFruits();
        fruitScore = player.fruitScore;
        fruits = player.fruits;
        maxFruits = player.maxFruits;

        speed = player.speed;
        jumpForce = player.jumpForce;
    }

    void GetActiveFruits()
    {
        GameObject[] remainingFruits = GameManager.instance.remainingFruits;
        
        activeFruits = new bool[remainingFruits.Length];

        for (int i = 0; i < remainingFruits.Length; i++)
        {
            activeFruits[i] = remainingFruits[i].GetComponent<Collider2D>().enabled;
        }
    }
}
