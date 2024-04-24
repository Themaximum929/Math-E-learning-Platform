using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CourseManager : MonoBehaviour
{
    // For return to main page
    public Button ReturnButton;
    public GameManager gameManager;

    // For outputting the chapter content
    public TMP_Text ChapterContent;
    public GameObject ChapterBackground;

    public Button NextButton;

    public Button choice1;
    public Button choice2;

    public Button return_button;

    public Image ContentImage;

    // Decide the return status
    string return_status = "main page";

    public AudioDirector audioDirector;
    public AzureTTS audioController;
    public CharacterAnimation characterAnimation;

    private void Start()
    {
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);

        // Load course from file
        string path = "Assets/course.txt";
        string[] lines = File.ReadAllLines(path);

        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject chapter_button;

        foreach (Transform child in transform)
        {
            if (child.gameObject != buttonTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(' ');
            string chapterName = parts[0];
            string status = (parts[1] == "1") ? "Read" : "Not Read";
            string avaliable = (parts[2] == "1") ? "Avaliable" : "Not Avaliable";
            
            // Instantiate button
            chapter_button = Instantiate(buttonTemplate, transform);
            chapter_button.transform.GetChild(0).GetComponent<TMP_Text>().text = chapterName.Replace("_", " ");
            chapter_button.transform.GetChild(1).GetComponent<TMP_Text>().text = status;

            // Change Color of the button according to the status
            Button buttonComponent = chapter_button.GetComponent<Button>();
            ColorBlock colorBlock = buttonComponent.colors;
            if (status == "Read")
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#7ED7C1FF", out myColor);
                colorBlock.normalColor = myColor;
            }
            else
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#FF8F8FFF", out myColor);
                colorBlock.normalColor = myColor;
            }
            buttonComponent.colors = colorBlock;

            // Change Colord og the button according to the avaliability
            if (avaliable == "Not Avaliable")
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#383838FF", out myColor);
                colorBlock.normalColor = myColor;
                buttonComponent.colors = colorBlock;

                chapter_button.transform.GetChild(1).GetComponent<TMP_Text>().text = avaliable ;
                chapter_button.GetComponent<Button>().interactable = false;
            }
            else
            {
                chapter_button.GetComponent<Button>().onClick.AddListener(() => ShowChapterContent(chapterName));
            }            
        }
        Destroy(buttonTemplate);

        ReturnButton.onClick.RemoveAllListeners();
        ReturnButton.onClick.AddListener(() => ReturnToMainPage(return_status));
    }

    private void Update()
    {

    }

    public void ShowCourses()
    {
        Start();
    }

    // Refresh the course buttons
    private void RefreshCourseButtons(string[] lines)
    {

        GameObject buttonTemplate = transform.GetChild(0).gameObject;

        // Remove all the old buttons
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.gameObject != buttonTemplate) // Ensure not to add the template
            {
                children.Add(child.gameObject);
            }
        }

        foreach (GameObject child in children)
        {
            Destroy(child);
        }

        GameObject chapter_button;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(' ');
            string chapterName = parts[0];
            string status = (parts[1] == "1") ? "Read" : "Not Read";
            string avaliable = (parts[2] == "1") ? "Avaliable" : "Not Avaliable";

            // Instantiate button
            chapter_button = Instantiate(buttonTemplate, transform);
            chapter_button.transform.GetChild(0).GetComponent<TMP_Text>().text = chapterName.Replace("_", " ");
            chapter_button.transform.GetChild(1).GetComponent<TMP_Text>().text = status;

            // Change Color of the button according to the status
            Button buttonComponent = chapter_button.GetComponent<Button>();
            ColorBlock colorBlock = buttonComponent.colors;
            if (status == "Read")
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#7ED7C1FF", out myColor);
                colorBlock.normalColor = myColor;
            }
            else
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#FF8F8FFF", out myColor);
                colorBlock.normalColor = myColor;
            }
            buttonComponent.colors = colorBlock;

            // Change Colord og the button according to the avaliability
            if (avaliable == "Not Avaliable")
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#383838FF", out myColor);
                colorBlock.normalColor = myColor;
                buttonComponent.colors = colorBlock;

                chapter_button.transform.GetChild(1).GetComponent<TMP_Text>().text = avaliable;
                chapter_button.GetComponent<Button>().interactable = false;
            }
            else
            {
                chapter_button.GetComponent<Button>().onClick.AddListener(() => ShowChapterContent(chapterName));
            }
        }
        Destroy(buttonTemplate);
    }


    // Unread -> Read
    private void ChangeChapterStatus(string chapterName, string newStatus)
    {
        string path = "Assets/course.txt";
        string[] lines = File.ReadAllLines(path);

        // Find the chapter and change its status
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(' ');
            if (parts[0] == chapterName)
            {
                lines[i] = chapterName + " " + newStatus + " 1";
                break;
            }
        }
        // Write the updated status back to the file
        File.WriteAllLines(path, lines);

        // Refresh the course buttons
        RefreshCourseButtons(lines);
    }

    private string[] chapterLines;
    private int lineIndex = 0;
    private bool question = false;
    private bool redirect = false;
    private bool before = false;


    // Show the content of each chapter
    private void ShowChapterContent(string chapterName)
    {
        chapterLines = null;
        lineIndex = 0;
        question = false;
        redirect = false;
        before = false;

        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);

        return_status = "chapter content";

        gameObject.SetActive(false);
        return_button.gameObject.SetActive(false);
        ChapterBackground.gameObject.SetActive(true);
        ChapterContent.gameObject.SetActive(true);



        NextButton.gameObject.SetActive(true);
        NextButton.onClick.RemoveAllListeners();
        NextButton.onClick.AddListener(() => DisplayNextLine(chapterName));
        // Load chapter content from file
        string path = "Assets/Chapters/" + chapterName + ".txt";
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            chapterLines = content.Split('\n');

            Debug.Log("Pass");
            DisplayNextLine(chapterName);
        }
        else
        {
            Debug.Log("File does not exist: " + path);
        }


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


    Sprite image = null;
    private void DisplayNextLine(string chapterName)
    {
        Debug.Log("lineIndex: " + lineIndex);
        Debug.Log("Question: " + question);
        Debug.Log("redirect: " + redirect);
        Debug.Log("before: " + before);


        if (lineIndex < chapterLines.Length)
        {
            // Display image
            string image_path = "Assets/Chapters/" + chapterName + "_image/";
            string[] image_files = Directory.GetFiles(image_path, "*.png");
            foreach (string filePath in image_files)
            {
                Debug.Log(filePath);
                Texture2D texture = LoadTexture(filePath);
                if (texture != null)
                {
                    string line = chapterLines[lineIndex].Trim();
                    if (line.Contains("?"))
                    {
                        if (filePath.Contains("question"))
                        {
                            before = true;
                            ContentImage.gameObject.SetActive(true);
                            image = Texture2DToSprite(texture);
                            ContentImage.sprite = image;
                            break;
                        }
                    }
                    
                    else if (question == false && before == true)
                    {
                        if (filePath.Contains("after") && redirect == false)
                        {
                            ContentImage.gameObject.SetActive(true);
                            image = Texture2DToSprite(texture);
                            ContentImage.sprite = image;
                            break;
                        }
                    }
                    else
                    {
                        // set active to false
                        ContentImage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError("Failed to load image: " + filePath);
                }
            }
        }


        // Display MC questions
        if (question == true)
        {
            choice1.gameObject.SetActive(true);
            choice2.gameObject.SetActive(true);

            string line = chapterLines[lineIndex].Trim();
            string temp = line.Replace("!", "");
            string[] parts = temp.Split('|');

            if (parts.Length >= 2)
            {
                choice1.GetComponentInChildren<TMP_Text>().text = parts[0];
                choice2.GetComponentInChildren<TMP_Text>().text = parts[1];
            }
            else
            {
                Debug.Log("Invalid format for multiple-choice question: " + line);
                return;
            }

            choice1.onClick.RemoveAllListeners();
            choice2.onClick.RemoveAllListeners();

            string[] parts2 = line.Split('|');
            choice1.onClick.AddListener(() => EvaluateAnswer(parts2[0], chapterName));
            choice2.onClick.AddListener(() => EvaluateAnswer(parts2[1], chapterName));
        }
        else if (lineIndex < chapterLines.Length)
        {
            choice1.gameObject.SetActive(false);
            choice2.gameObject.SetActive(false);

            string line = chapterLines[lineIndex].Trim();

            if (line.Contains("?"))
            {
                question = true;
            }

            ChapterContent.text = line;

            audioDirector.stop_director();
            audioDirector.talk_director(line);

            if (redirect == true)
            {         
                lineIndex -= 2;
                redirect = false;
            }
            else
            {
                Debug.Log("Pass2");              
                lineIndex++;
            }
        }
        else
        {
            choice1.gameObject.SetActive(false);
            choice2.gameObject.SetActive(false);

            ChangeChapterStatus(chapterName, "1");
            ChapterContent.text = "End of chapter";
            ContentImage.gameObject.SetActive(false);
            audioDirector.talk_director(ChapterContent.text);
            NextButton.gameObject.SetActive(false);
            return_button.gameObject.SetActive(true);
            return_button.onClick.RemoveAllListeners();
            return_button.onClick.AddListener(() => ReturnToMainPage("chapter content"));
        }


    }

    private void EvaluateAnswer(string answer, string chapterName)
    {
        if (answer.Contains("!"))
        {
            characterAnimation.Character_Animation("nod", "default");
            lineIndex += 2;
        }
        else
        {
            characterAnimation.Character_Animation("pon", "default");
            lineIndex++;
            redirect = true;
        }
        question = false;

        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);

        DisplayNextLine(chapterName);
    }

    // Return button
    public void ReturnToMainPage(string message)
    {
        if (message == "main page")
        {
            characterAnimation.Character_Animation("default", "default");
            audioDirector.stop_director();
            audioDirector.talk_director("Return to main menu");
            gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            ChapterContent.gameObject.SetActive(false);
        }
        if (message == "chapter content")
        {
            characterAnimation.Character_Animation("default", "default");
            audioDirector.stop_director();
            audioDirector.talk_director("Return to chapter list");
            return_status = "main page";
            ChapterContent.gameObject.SetActive(false);
            ChapterBackground.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }   

    }
 }
