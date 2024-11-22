using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
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
        finalScoreText.text = "Score: " + other.GetComponent<PlayerSP>().GetScore();
        yield return new WaitForSeconds(scoreTime);
        finalScoreText.text = "Health Boost: " + other.GetComponent<PlayerSP>().GetHealthScore() * 100 + "%";
        yield return new WaitForSeconds(scoreTime);
        finalScoreText.text = "Final Score: " + other.GetComponent<PlayerSP>().GetFinalScore();
        yield return new WaitForSeconds(scoreTime);
        finalScoreText.text = "Loading Next Level";
        yield return new WaitForSeconds(scoreTime);
        NextLevel();
    }
    void NextLevel()
    {
        if (scene.buildIndex + 1 == 3)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(scene.buildIndex + 1);
        }
    }
}
