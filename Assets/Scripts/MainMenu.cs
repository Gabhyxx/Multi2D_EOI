using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Image imageCharacter;
    public int characterIndex;
    public Sprite[] characterSprites;

    [SerializeField] TextMeshProUGUI splashText;
    [SerializeField] string[] splashes;
    [SerializeField] Button loadButton;

    private void Start()
    {
        string path = Application.persistentDataPath + "/player.gbx";

        if (File.Exists(path) == false) 
        {
            loadButton.interactable = false;
        }

        RandomSplash();
    }
    public void IncreaseCharacter()
    {
        characterIndex++;
        if (characterIndex >= characterSprites.Length)
        {
            characterIndex = 0;
        }
        imageCharacter.sprite = characterSprites[characterIndex];
        GameManager.instance.playerNumber = characterIndex;
    }
    public void DecreaseCharacter()
    {
        characterIndex--;
        if (characterIndex <= 0)
        {
            characterIndex = characterSprites.Length - 1;
        }
        imageCharacter.sprite = characterSprites[characterIndex];
        GameManager.instance.playerNumber = characterIndex;
    }
    void RandomSplash()
    {
        splashText.text = splashes[Random.Range(0, splashes.Length)];
    }
}
