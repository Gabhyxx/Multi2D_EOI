using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZoneMP : MonoBehaviour
{
    [SerializeField] TextMeshPro finalScoreText;
    [SerializeField] float scoreTime;

    Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FinalScoreDisplay(other));
        }
    }
    IEnumerator FinalScoreDisplay(Collider2D other)
    {
        yield return new WaitForSeconds(1);
        finalScoreText.text = "Score: " + other.GetComponent<Player>().GetScore();
        yield return new WaitForSeconds(scoreTime);
        finalScoreText.text = "Health Boost: " + other.GetComponent<Player>().GetHealthScore() * 100 + "%";
        yield return new WaitForSeconds(scoreTime);
        finalScoreText.text = "Final Score: " + other.GetComponent<Player>().GetFinalScore();
        yield return new WaitForSeconds(scoreTime);
        finalScoreText.text = "Returning to Main Menu";
        yield return new WaitForSeconds(scoreTime);
        NextLevel();
    }
    void NextLevel()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
