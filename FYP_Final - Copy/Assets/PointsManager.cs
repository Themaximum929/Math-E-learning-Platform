using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class PointsManager : MonoBehaviour
{
    public PointsPanel pointsPanel;
    public DatabaseManager databaseManager;
    public GameManager gameManager;

    private void Start()
    {

        // Load Chapter from file
        string path = "Assets/course.txt";
        string[] lines = File.ReadAllLines(path);
        Debug.Log(lines);

        GameObject contentPanel = this.gameObject;

        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject chapter_button;

        foreach (Transform child in transform)
        {
            if (child.gameObject != buttonTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (string line in lines)
        {
            string[] parts = line.Split(' ');
            string chapterName = parts[0];
            string status = (parts[2] == "0") ? "100 Pts" : "Bought";

            chapter_button = Instantiate(buttonTemplate, transform);
            chapter_button.SetActive(true);

            // Change Color of the button according to the status
            Button buttonComponent = chapter_button.GetComponent<Button>();
            ColorBlock colorBlock = buttonComponent.colors;
            if (status == "Bought")
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#222222FF", out myColor);
                colorBlock.normalColor = myColor;
            }
            else
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#FFC107FF", out myColor);
                colorBlock.normalColor = myColor;
            }
            buttonComponent.colors = colorBlock;

            chapter_button.transform.GetChild(0).GetComponent<TMP_Text>().text = chapterName.Replace("_"," ");
            chapter_button.transform.GetChild(1).GetComponent<TMP_Text>().text = status;

            if (status == "100 Pts")
            {
                chapter_button.GetComponent<Button>().onClick.AddListener(() => ConfirmPurchase(chapterName));

            }
        }
        Destroy(buttonTemplate);
    }

    public void refresh_course_list(string Obj_name)
    {
        if (Obj_name.Contains("Chapter"))
        {
            Obj_name = Obj_name.Split(' ')[1];  
        }
        string path = "Assets/course.txt";
        string[] lines = File.ReadAllLines(path);

        foreach (Transform child in transform)
        {
            TMP_Text chapterNameText = child.transform.GetChild(0).GetComponent<TMP_Text>();
            Button buttonComponent = child.GetComponent<Button>();
            ColorBlock colorBlock = buttonComponent.colors;
            if (chapterNameText.text.Replace(" ", "_") == Obj_name)
            {
                // Change color
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#222222FF", out myColor);
                colorBlock.normalColor = myColor;
                buttonComponent.colors = colorBlock;

                // Change status
                child.transform.GetChild(1).GetComponent<TMP_Text>().text = "Bought";

                // Remove the listener
                buttonComponent.onClick.RemoveAllListeners();
                break;
            }
        }

        // Find the chapter and change its status
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(' ');
            if (parts[0] == Obj_name)
            {
                lines[i] = Obj_name + " 0 1";
                break;
            }
        }
        // Write the updated status back to the file
        File.WriteAllLines(path, lines);

        gameManager.update_points(-100);
    }


    private void ConfirmPurchase(string ChapterName)
    {
        Debug.Log("Chapter Pressed: " + ChapterName);
        pointsPanel.confirmation("chapter", "Chapter: " + ChapterName, 100);
    }

}