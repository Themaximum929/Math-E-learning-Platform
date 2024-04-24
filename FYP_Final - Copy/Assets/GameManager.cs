using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpeechLib;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    // For chatbox
    public string username;
    public int max_message = 25;
    public GameObject chatboxPanel;
    public GameObject chatPanel, textObject;
    public Button Close_chatbox;
    public TMP_InputField chatBox;
    public Color playerMessage, info;

    public Button Chatbox_button;

    // For main menu
    public Button Quiz_Button;
    public Button Course_Button;
    public Button Points_button;

    [SerializeField]
    List<Message> message_list = new List<Message>();

    public CourseManager courseManager;
    public QuizManager quizManager;
    public IndividualQuiz individualQuiz;
    public DatabaseManager databaseManager;
    public PointsPanel pointsPanel;


    public AzureTTS audioController;

    public AI_algorithm AI_algorithm;


    // Start is called before the first frame update
    void Start()
    {
        databaseManager.SavePoints(350); 

        Points_button.onClick.AddListener(Points_Button_Click);
        Quiz_Button.onClick.AddListener(Quiz_Button_Click);
        Course_Button.onClick.AddListener(Course_Button_Click);
        Chatbox_button.onClick.AddListener(Chatbox_button_Click);
        Close_chatbox.onClick.AddListener(Close_chatbox_Click);

        // Deactivate Course Panel
        courseManager.gameObject.SetActive(false);
        courseManager.ReturnButton.gameObject.SetActive(false);
        courseManager.ChapterContent.gameObject.SetActive(false);
        courseManager.ChapterBackground.gameObject.SetActive(false);


        // Deactivate Quiz Panel
        quizManager.gameObject.SetActive(false);
        quizManager.ReturnButton.gameObject.SetActive(false);
        quizManager.QuizContent.gameObject.SetActive(false);
        quizManager.QuizBackground.gameObject.SetActive(false);

        // Deactivate Individual Quiz Panel
        individualQuiz.gameObject.SetActive(false);
        individualQuiz.OptionButton1.gameObject.SetActive(false);
        individualQuiz.OptionButton2.gameObject.SetActive(false);
        individualQuiz.OptionButton3.gameObject.SetActive(false);
        individualQuiz.OptionButton4.gameObject.SetActive(false);
        individualQuiz.QuestionText.gameObject.SetActive(false);
        individualQuiz.ScoreText.gameObject.SetActive(false);

        // Deactivate Chatbox
        chatboxPanel.gameObject.SetActive(false);

        // Deactivate Points Panel
        pointsPanel.gameObject.SetActive(false);

        // Get points text and save it to button
        Points_button.GetComponentInChildren<TMP_Text>().text = databaseManager.GetPoints().ToString();
    }

    void Points_Button_Click()
    {
        // Deactivate Individual Quiz Panel
        individualQuiz.gameObject.SetActive(false);
        individualQuiz.OptionButton1.gameObject.SetActive(false);
        individualQuiz.OptionButton2.gameObject.SetActive(false);
        individualQuiz.OptionButton3.gameObject.SetActive(false);
        individualQuiz.OptionButton4.gameObject.SetActive(false);
        individualQuiz.QuestionText.gameObject.SetActive(false);
        individualQuiz.ScoreText.gameObject.SetActive(false);

        // Deactivate Course Panel
        courseManager.gameObject.SetActive(false);
        courseManager.ReturnButton.gameObject.SetActive(false);
        courseManager.ChapterContent.gameObject.SetActive(false);
        courseManager.ChapterBackground.gameObject.SetActive(false);

        // Deactivate Quiz Panel
        quizManager.gameObject.SetActive(false);
        quizManager.ReturnButton.gameObject.SetActive(false);
        quizManager.QuizContent.gameObject.SetActive(false);
        quizManager.QuizBackground.gameObject.SetActive(false);

        //TODO
        //databaseManager.SavePoints(0);
        //Points_button.GetComponentInChildren<TMP_Text>().text = databaseManager.GetPoints().ToString();
        int temp = databaseManager.GetPoints();

        audioController.StopAudio();
        StartCoroutine(audioController.ConvertTextToSpeech("Points panel. You now have " + temp + "points available"));
        pointsPanel.points_panel();
    }

    public void update_points(int add_point)
    {
        int temp = databaseManager.GetPoints();
        databaseManager.SavePoints(temp + add_point);
        Points_button.GetComponentInChildren<TMP_Text>().text = databaseManager.GetPoints().ToString();
    }

    void Close_chatbox_Click()
    {
        chatboxPanel.gameObject.SetActive(false);
        Chatbox_button.gameObject.SetActive(true);
    }
    void Chatbox_button_Click()
    {
        Chatbox_button.gameObject.SetActive(false);
        audioController.StopAudio();
        StartCoroutine(audioController.ConvertTextToSpeech("Chatbox menu."));
        //SendMessageToChat("You have Pressed Chatbox Button", Message.MessageType.info);
        chatboxPanel.gameObject.SetActive(true);
    }

        void Quiz_Button_Click()
    {
        // Deactivate Individual Quiz Panel
        individualQuiz.gameObject.SetActive(false);
        individualQuiz.OptionButton1.gameObject.SetActive(false);
        individualQuiz.OptionButton2.gameObject.SetActive(false);
        individualQuiz.OptionButton3.gameObject.SetActive(false);
        individualQuiz.OptionButton4.gameObject.SetActive(false);
        individualQuiz.QuestionText.gameObject.SetActive(false);
        individualQuiz.ScoreText.gameObject.SetActive(false);

        // Deactivate Course Panel
        courseManager.gameObject.SetActive(false);
        courseManager.ReturnButton.gameObject.SetActive(false);
        courseManager.ChapterContent.gameObject.SetActive(false);
        courseManager.ChapterBackground.gameObject.SetActive(false);


        // Deactivate Points Panel
        pointsPanel.gameObject.SetActive(false);

        audioController.StopAudio();
        StartCoroutine(audioController.ConvertTextToSpeech("Quiz menu."));

        quizManager.gameObject.SetActive(true);
        quizManager.ReturnButton.gameObject.SetActive(true);
        quizManager.ShowQuiz();
    }

    // Action when course button is clicked
    
    void Course_Button_Click()
    {
        // Deactivate Individual Quiz Panel
        individualQuiz.gameObject.SetActive(false);
        individualQuiz.OptionButton1.gameObject.SetActive(false);
        individualQuiz.OptionButton2.gameObject.SetActive(false);
        individualQuiz.OptionButton3.gameObject.SetActive(false);
        individualQuiz.OptionButton4.gameObject.SetActive(false);
        individualQuiz.QuestionText.gameObject.SetActive(false);
        individualQuiz.ScoreText.gameObject.SetActive(false);

        // Deactivate Quiz Panel
        quizManager.gameObject.SetActive(false);
        quizManager.ReturnButton.gameObject.SetActive(false);
        quizManager.QuizContent.gameObject.SetActive(false);
        quizManager.QuizBackground.gameObject.SetActive(false);

        // Deactivate Points Panel
        pointsPanel.gameObject.SetActive(false);

        audioController.StopAudio();
        StartCoroutine(audioController.ConvertTextToSpeech("Course menu."));

        courseManager.gameObject.SetActive(true);
        courseManager.ChapterBackground.gameObject.SetActive(false);
        courseManager.ReturnButton.gameObject.SetActive(true);
        courseManager.ShowCourses();
    }

    // Update is called once per frame
    void Update()
    {
        if (chatBox.text != "")
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                //ChatBot Chatbot = new ChatBot();

                SendMessageToChat(username + ": " + chatBox.text, Message.MessageType.playerMessage);
                Debug.Log("Player: " + chatBox.text);


                StartCoroutine(AI_algorithm.AI_responseCoroutine(chatBox.text, (response) =>
                {
                    SendMessageToChat(response, Message.MessageType.info);
                }));

                chatBox.text = "";
            }
        }
        else
        {
            if (!chatBox.isFocused && UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }

        if (!chatBox.isFocused)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                SendMessageToChat("You have Pressed Space", Message.MessageType.info);
                Debug.Log("Space");
            }
        }

    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if (message_list.Count >= max_message)
        {
            Destroy(message_list[0].textObject.gameObject);
            message_list.Remove(message_list[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        // Set the text object as active
        newText.SetActive(true);

        message_list.Add(newMessage);
    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = info;

        switch (messageType)
        {
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
        }

        return color;
    }


}

[System.Serializable]
public class Message
{
    public string text;
    public TMP_Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        info
    }
}