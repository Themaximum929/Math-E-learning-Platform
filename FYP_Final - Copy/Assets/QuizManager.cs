using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    // For return to main page
    public Button ReturnButton;
    public GameManager gameManager;

    // For outputting the quiz content
    public TMP_Text QuizContent;
    public GameObject QuizBackground;

    // Decide the return status

    public IndividualQuiz individualQuiz;

    public AudioDirector audioDirector;

    public DatabaseManager databaseManager;


    public void Start()
    {
        // Load quiz from file
        string path = "Assets/quiz.txt";
        string[] lines = File.ReadAllLines(path);

        string cousrepath = "Assets/course.txt";
        string[] course = File.ReadAllLines(cousrepath);

        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject quiz_button;

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
            string quizName = parts[0];
            string status = (parts[1] == "1") ? "Complete" : "Incomplete";

            string string_display = "";
            // Show Highest score if Complete otherwise show incomplete
            if (status == "Complete")
            {
                string_display = "Highest Score: " + databaseManager.GetHighScore(quizName).ToString() +  "/10"; 
            }
            else
            {
                string_display = status;
            }

            string[] courseparts = course[i].Split(' ');
            string avaliable = (courseparts[2] == "1") ? "Avaliable" : "Not Avaliable";

            // Instantiate button
            quiz_button = Instantiate(buttonTemplate, transform);
            quiz_button.transform.GetChild(0).GetComponent<TMP_Text>().text = quizName.Replace("_", " ");
            quiz_button.transform.GetChild(1).GetComponent<TMP_Text>().text = string_display;

            // Change Color of the button according to the status
            Button buttonComponent = quiz_button.GetComponent<Button>();
            ColorBlock colorBlock = buttonComponent.colors;
            if (status == "Complete")
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
                databaseManager.SaveDifficulty(quizName, 1);
            }
            buttonComponent.colors = colorBlock;

            // Change Colord og the button according to the avaliability
            if (avaliable == "Not Avaliable")
            {
                Color myColor = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString("#383838FF", out myColor);
                colorBlock.normalColor = myColor;
                buttonComponent.colors = colorBlock;

                quiz_button.transform.GetChild(1).GetComponent<TMP_Text>().text = avaliable;
                quiz_button.GetComponent<Button>().interactable = false;
            }
            else
            {
                quiz_button.GetComponent<Button>().onClick.AddListener(() => ShowQuizContent(quizName));
            }
        }
        Destroy(buttonTemplate);

        ReturnButton.onClick.RemoveAllListeners();
        ReturnButton.onClick.AddListener(() => ReturnToMainPage("main menu"));
    }

    public void ShowQuiz()
    {
        Start();
    }

    public void ChangeQuizStatus(string quizName, string newStatus)
    {
        string path = "Assets/quiz.txt";
        string[] lines = File.ReadAllLines(path);


        // Find the quiz and change its status
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(' ');
            if (parts[0] == quizName)
            {
                lines[i] = quizName + " " + newStatus;
                break;
            }
        }
        // Write the updated status back to the file
        File.WriteAllLines(path, lines);

        // Refresh the quiz buttons
        RefreshQuizButtons(lines);
    }

    private void RefreshQuizButtons(string[] lines)
    {
            string coursePath = "Assets/course.txt";
            string[] course = File.ReadAllLines(coursePath);

            GameObject buttonTemplate = transform.GetChild(0).gameObject;

            Debug.Log("Using template: " + buttonTemplate.name);

            // Remove all old buttons
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

            GameObject quiz_button;

            // Instantiate new buttons
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(' ');
                string quizName = parts[0];
                string status = (parts[1] == "1") ? "Complete" : "Incomplete";

                string string_display = "";
                // Show Highest score if Complete otherwise show incomplete
                if (status == "Complete")
                {
                    string_display = "Highest Score: " + databaseManager.GetHighScore(quizName).ToString() + "/10";
                }
                else
                {
                    string_display = status;
                }

                string[] courseparts = course[i].Split(' ');
                string avaliable = (courseparts[2] == "1") ? "Avaliable" : "Not Avaliable";

                // Instantiate button
                quiz_button = Instantiate(buttonTemplate, transform);
                quiz_button.transform.GetChild(0).GetComponent<TMP_Text>().text = quizName.Replace("_", " ");
                quiz_button.transform.GetChild(1).GetComponent<TMP_Text>().text = string_display;

                // Change Color of the button according to the status
                Button buttonComponent = quiz_button.GetComponent<Button>();
                ColorBlock colorBlock = buttonComponent.colors;
                if (status == "Complete")
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
                    databaseManager.SaveDifficulty(quizName, 1);
                }
                buttonComponent.colors = colorBlock;

                // Change Colord og the button according to the avaliability
                if (avaliable == "Not Avaliable")
                {
                    Color myColor = new Color();
                    UnityEngine.ColorUtility.TryParseHtmlString("#383838FF", out myColor);
                    colorBlock.normalColor = myColor;
                    buttonComponent.colors = colorBlock;

                    quiz_button.transform.GetChild(1).GetComponent<TMP_Text>().text = avaliable;
                    quiz_button.GetComponent<Button>().interactable = false;
                }
                else
                {
                    quiz_button.GetComponent<Button>().onClick.AddListener(() => ShowQuizContent(quizName));
                }
            }
            Destroy(buttonTemplate);

            ReturnButton.onClick.AddListener(() => ReturnToMainPage("main menu"));
    }
    

    public void ShowQuizContent(string Q_name)
    {
 
        ReturnButton.gameObject.SetActive(false);

        audioDirector.stop_director();
        string temp = Q_name.Replace("_", " ");
        temp = temp.Replace("I", "one");
        audioDirector.talk_director("You have selected quiz chapter: " + temp + ". Let's start the quiz!");
        gameObject.SetActive(false);
        individualQuiz.QuizInitialize(Q_name);
    }

    public void ReturnToMainPage(string text)
    {
        if (text == "main menu")
        {
            audioDirector.talk_director("Return to main menu");
            gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);

            gameManager.Quiz_Button.gameObject.SetActive(true);
            gameManager.Course_Button.gameObject.SetActive(true);
        }
    }
}

