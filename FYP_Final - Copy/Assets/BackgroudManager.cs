using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class BackgroundManager : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }
    public Image mainCanvasBackground;

    public PointsPanel pointsPanel;
    public DatabaseManager databaseManager;
    public GameManager gameManager;

    private void Start()
    {
        background_loading();
        initialize_background();
    }

    private void initialize_background()
    {
        string Obj_name = databaseManager.GetBackground();
        Debug.Log(Obj_name);
        if (Obj_name.Contains("Background"))
        {
            Obj_name = Obj_name.Split(' ')[1];
        }

        string filePath = "Assets/background/" + Obj_name + ".jpg";

        Texture2D texture = LoadTexture(filePath);
        if (texture != null) 
        {
            Sprite sprite = Texture2DToSprite(texture);
            mainCanvasBackground.sprite = sprite;
        }
        else
        {
            Debug.LogError("Failed to load background: " + Obj_name);
        }
    }

    private void background_loading()
    {
        // Load Chapter from file
        string path = "Assets/background/";
        string[] files = Directory.GetFiles(path, "*.jpg");

        GameObject contentPanel = this.gameObject;

        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject background_button;

        foreach (string filePath in files)
        {
            background_button = Instantiate(buttonTemplate, transform);
            background_button.SetActive(true);

            Texture2D texture = LoadTexture(filePath);

            string backgroundName = Path.GetFileNameWithoutExtension(filePath);

            if (texture == null) continue;

            Sprite sprite = Texture2DToSprite(texture);

            background_button.GetComponent<Image>().sprite = sprite;

            background_button.transform.GetChild(0).GetComponent<TMP_Text>().text = "10 Pts";

            background_button.GetComponent<Button>().onClick.AddListener(() => ConfirmPurchase(backgroundName));

        }
        Destroy(buttonTemplate);
    }

    // Load images to button
    private Texture2D LoadTexture(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }

    private Sprite Texture2DToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    // Action after buying the product
    public void after_buying_background(string Obj_name)
    {
        if (Obj_name.Contains("Background"))
        {
            Obj_name = Obj_name.Split(' ')[1];
        }

        string filePath = "Assets/background/" + Obj_name + ".jpg";

        Texture2D texture = LoadTexture(filePath);
        if (texture != null)
        {
            Sprite sprite = Texture2DToSprite(texture);
            mainCanvasBackground.sprite = sprite;
        }
        else
        {
            Debug.LogError("Failed to load background: " + Obj_name);
        }

        // Save background
        databaseManager.SaveBackground(Obj_name);

        // Update points
        gameManager.update_points(-10);
    }

    // Open confirmation panel
    private void ConfirmPurchase(string backgroundName)
    {
        pointsPanel.confirmation("background","Background: " + backgroundName, 10);
    }
}
