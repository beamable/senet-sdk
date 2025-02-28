using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class MoreGamesManager : MonoBehaviour
{
    private const string BASE_ICON_PATH = "Packages/com.beamable.senetsdk/Runtime/Assets/Scenes/Assets/Senet Assets/More Games Icons/";

    [System.Serializable]
    public class GameInfo
    {
        public string gameName;
        public Sprite gameIcon;
        public string gameURL;

        public GameInfo(string name, string iconFileName, string url)
        {
            gameName = name;
            gameIcon = LoadSpriteFromPath(BASE_ICON_PATH + iconFileName);
            gameURL = url;
        }
    }

    public GameObject gamePrefab;  
    public Transform contentParent;  

    private List<GameInfo> gamesList = new List<GameInfo>();

    private void Start()
    {
        LoadDummyGames();
        PopulateGames();
    }

    private void LoadDummyGames()
    {
        gamesList = new List<GameInfo>
        {
            new GameInfo("Number Quest", "NumberQuest.png", ""),
            new GameInfo("Bang Bubs", "BangBubs.png", "")
        };
    }

    private void PopulateGames()
    {
        foreach (var game in gamesList)
        {
            GameObject newGameItem = Instantiate(gamePrefab, contentParent);

            Image gameImage = newGameItem.transform.Find("Image").GetComponent<Image>();
            gameImage.sprite = game.gameIcon;
            gameImage.preserveAspect = true;  

            newGameItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = game.gameName;

            // newGameItem.GetComponent<Button>().onClick.AddListener(() => OpenGame(game.gameURL)); // Future use
        }
    }


    private static Sprite LoadSpriteFromPath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Sprite not found at path: {filePath}");
            return null;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);

        if (!texture.LoadImage(fileData))
        {
            Debug.LogWarning($"Failed to load image: {filePath}");
            return null;
        }

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    // Future functionality
    // private void OpenGame(string gameURL) { }
}
