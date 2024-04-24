using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System;

public class IndividualQuiz : MonoBehaviour
{
    // MC question
    public TMP_Text QuestionText;
    // MC options
    public Button OptionButton1;
    public Button OptionButton2;
    public Button OptionButton3;
    public Button OptionButton4;

    // Drag

    // MC panel
    public GameObject MCPanel;
    // Drag Panel
    public GameObject DragPanel;
    // Inequality Panel
    public GameObject InequalityPanel;

    // For displaying the score
    public TMP_Text ScoreText;
    public TMP_Text Points;

    // Progress bar
    public Image ProgressBar;
    public Image Bar_background;

    // Hints
    public Button HintButton;
    public Button HintText;

    private int score = 0;

    public AudioDirector audioDirector;
    public Button Indi_quizReturnButton;

    public QuizManager quizManager;
    public DragDropQuestion dragDropQuestion;
    public InequalityDrag inequalityDrag;
    public CharacterAnimation characterAnimation;
    public DatabaseManager databaseManager;
    public GameManager GameManager;
    public AI_algorithm aI_Algorithm;

    int question_count = 0;
    string general_quiz;

    private int currentQuestionIndex = 0;


    private void Start()
    {
        Indi_quizReturnButton.onClick.AddListener(() => ReturnToMainPage("quiz panel"));
        ProgressBar.fillAmount = 0f;

        HintText.gameObject.SetActive(false);
    }

    // called from QuizManager
    public void QuizInitialize(string quiz_name)
    {
        question_count = 0;
        currentQuestionIndex = 0;
        score = 0;

        gameObject.SetActive(true);
        OptionButton1.gameObject.SetActive(true);
        OptionButton2.gameObject.SetActive(true);
        OptionButton3.gameObject.SetActive(true);
        OptionButton4.gameObject.SetActive(true);
        QuestionText.gameObject.SetActive(true);
        Indi_quizReturnButton.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(false);
        Points.gameObject.SetActive(false);

        //LoadMC(quiz_name);

        OptionButton1.onClick.RemoveAllListeners();
        OptionButton1.onClick.AddListener(() => CheckAnswer("Option1", quiz_name));

        OptionButton2.onClick.RemoveAllListeners();
        OptionButton2.onClick.AddListener(() => CheckAnswer("Option2", quiz_name));

        OptionButton3.onClick.RemoveAllListeners();
        OptionButton3.onClick.AddListener(() => CheckAnswer("Option3", quiz_name));

        OptionButton4.onClick.RemoveAllListeners();
        OptionButton4.onClick.AddListener(() => CheckAnswer("Option4", quiz_name));

        DisplayQuestion(quiz_name);
    }

    private void DisplayQuestion(string quiz_name)
    {
        // Preset difficulty level
        int diff =  databaseManager.GetDifficulty(quiz_name);
        if (diff == 0)
        {
            diff = 1;
            databaseManager.SaveDifficulty(quiz_name, diff); 
        }
        Debug.Log("Difficulty Level: " + diff);
       

        // Update Progress bar
        float progress = (float)question_count / 10f;
        ProgressBar.fillAmount = progress;

        // Randomly choose questioning style
        if (question_count < 10)
        {
            HintButton.gameObject.SetActive(true);
            characterAnimation.Character_Animation("default", "default");

            HintText.gameObject.SetActive(false);
            
            System.Random rnd = new System.Random();
            int questionStyle = rnd.Next(0, 3);
            //int questionStyle = 0;

            if (quiz_name != "Addition" && quiz_name != "Subtraction" && quiz_name != "Multiplication" && quiz_name != "Division")
            {
                questionStyle = 0;
            }

            switch (questionStyle)
            {
                case 0:
                    MCPanel.gameObject.SetActive(true);
                    DragPanel.gameObject.SetActive(false);
                    InequalityPanel.gameObject.SetActive(false);
                    question_count++;
                    DisplayMCQuestion(quiz_name, question_count, diff);
                    break;
                case 1:
                    MCPanel.gameObject.SetActive(false);
                    DragPanel.gameObject.SetActive(true);
                    InequalityPanel.gameObject.SetActive(false);
                    question_count++;
                    dragDropQuestion.DisplayDragDropQuestion(quiz_name, question_count, diff );
                    break;
                case 2:
                    MCPanel.gameObject.SetActive(false);
                    DragPanel.gameObject.SetActive(false);
                    InequalityPanel.gameObject.SetActive(true);
                    question_count++;
                    inequalityDrag.DisplayInequalityQuestion(quiz_name, question_count, diff);
                    break;
                default:
                    throw new Exception("Invalid Question Style");
            }
        }
        else
        {
            HintText.gameObject.SetActive(false);
            HintButton.gameObject.SetActive(false);

            MCPanel.gameObject.SetActive(false);
            DragPanel.gameObject.SetActive(false);
            InequalityPanel.gameObject.SetActive(false);

            // Add the score
            Points.text = "You earned " + score * diff + " points!";

            // Quiz ended, display score and return button, hide option buttons and question text
            ScoreText.text = "Your Final Score Is: " + score;

            int old_high_score = databaseManager.GetHighScore(quiz_name);
            if (old_high_score < score)
            {
                databaseManager.SaveHighScore(quiz_name, score);
            }

            if (score == 10)
            {
                audioDirector.talk_director("Incredible! You scored a perfect " + score + ". Keep up the great work!");
                databaseManager.SaveDifficulty(quiz_name, diff + 1);

                characterAnimation.Character_Animation("special1", "kirara");
            }
            else if (score >= 7)
            {
                audioDirector.talk_director("Great job! You scored " + score + ". You're doing really well!");

                characterAnimation.Character_Animation("special1", "default");
            }
            else if (score >= 5)
            {
                audioDirector.talk_director("Good effort! You scored " + score + ". Keep practicing and you'll get even better!");

                characterAnimation.Character_Animation("swing", "default");
            }
            else
            {
                audioDirector.talk_director("You scored " + score + ". Don't worry, keep trying and you'll improve!");
                databaseManager.SaveDifficulty(quiz_name, diff - 1);

                characterAnimation.Character_Animation("special2", "sad");
            }
            ScoreText.gameObject.SetActive(true);
            Points.gameObject.SetActive(true);
            Indi_quizReturnButton.gameObject.SetActive(true);

            quizManager.ChangeQuizStatus(quiz_name, "1");    // Mark the quiz as "Complete"
            // Update points
            GameManager.update_points(score * diff);
            //quizManager.Start();
        }
    }

    // Function that receive the user is correct or not 
    public void DragDropOnUserAnswered(string quiz_name, bool isCorrect, string speech = "")
    {
        if (isCorrect)
        {
            characterAnimation.Character_Animation("nod", "happy");

            score++;
            AudioSpeech(1, quiz_name, speech);
            StartCoroutine(WaitAndLoadNextQuestionDrop(5, quiz_name));
        }
        else
        {
            characterAnimation.Character_Animation("swing", "sad");

            AudioSpeech(2, quiz_name, speech);
            StartCoroutine(WaitAndLoadNextQuestionDrop(7, quiz_name));
        }
    }


    private int correctAnswer;
    private string general_correctAnswer;
    private void DisplayMCQuestion(string quiz_name, int question_count, int diff)
    {
        System.Random rnd = new System.Random();
        // General non artihmetic quiz
        if (quiz_name != "Addition" && quiz_name != "Subtraction" && quiz_name != "Multiplication" && quiz_name != "Division")
        {
            string[] lines = File.ReadAllLines($"Assets/Quiz_Question/{quiz_name}.txt");
            if (lines.Length > 0)
            {
                string selectedLine = lines[rnd.Next(lines.Length)];
                string[] parts = selectedLine.Split('|');
                Debug.Log(selectedLine);    

                if (parts.Length == 3)
                {
                    // Set the question text
                    QuestionText.text = "Q" + question_count + ". " + parts[0];

                    save_question(QuestionText.text, "MC");

                    // Set the correct answer
                    general_correctAnswer = parts[1];

                    // Set wrong answers
                    string[] wrongAnswers = parts[2].Split(',');

                    // Create an array of options including the correct answer
                    string[] options = new string[4];
                    options[0] = general_correctAnswer;
                    for (int i = 0; i < wrongAnswers.Length; i++)
                    {
                        options[i + 1] = wrongAnswers[i];
                    }

                    // Shuffle the array
                    options = options.OrderBy(x => rnd.Next()).ToArray();

                    // Display the options
                    float newFontSize = 45.0f;
                    OptionButton1.GetComponentInChildren<TMP_Text>().text = options[0];
                    OptionButton1.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
                    OptionButton2.GetComponentInChildren<TMP_Text>().text = options[1];
                    OptionButton2.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
                    OptionButton3.GetComponentInChildren<TMP_Text>().text = options[2];
                    OptionButton3.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
                    OptionButton4.GetComponentInChildren<TMP_Text>().text = options[3];
                    OptionButton4.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
                }
                else
                {
                    throw new Exception("Invalid question format in file.");
                }
            }
            else
            {
                throw new Exception("No questions found in file.");
            }

        }
        else
        {
            // Generate a random question 
            int num1, num2;

            general_quiz = quiz_name;

            switch (quiz_name)
            {
                case "Addition":
                    num1 = rnd.Next(1, 10 * diff);
                    num2 = rnd.Next(1, 10 * diff);
                    correctAnswer = num1 + num2;
                    QuestionText.text = "Q" + question_count + ". What is " + num1 + "+" + num2 + "?";
                    break;
                case "Subtraction":
                    num1 = rnd.Next(1, 10 * diff);
                    num2 = rnd.Next(1, 10 * diff);
                    correctAnswer = num1 - num2;
                    QuestionText.text = "Q" + question_count + ". What is " + num1 + "-" + num2 + "?";
                    break;
                case "Multiplication":
                    num1 = rnd.Next(1, 10 * (int)diff / 2);
                    num2 = rnd.Next(1, 10 * (int)diff / 2);
                    correctAnswer = num1 * num2;
                    QuestionText.text = "Q" + question_count + ". What is " + num1 + "x" + num2 + "?";
                    break;
                case "Division":
                    num2 = rnd.Next(1, 10 * (int)diff / 2);
                    int tempAnswer = rnd.Next(1, 10 * (int)diff / 2);
                    num1 = num2 * tempAnswer;
                    correctAnswer = num1 / num2;
                    QuestionText.text = $"What is {num1} ÷ {num2}?";
                    break;
                default:
                    int random = rnd.Next(1, 4);
                    switch (random)
                    {
                        case (1):
                            num1 = rnd.Next(1, 10 * diff);
                            num2 = rnd.Next(1, 10 * diff);
                            correctAnswer = num1 + num2;
                            QuestionText.text = "Q" + question_count + ". What is " + num1 + "+" + num2 + "?";
                            general_quiz = "Addition";
                            break;
                        case (2):
                            num1 = rnd.Next(1, 10 * diff);
                            num2 = rnd.Next(1, 10 * diff);
                            correctAnswer = num1 - num2;
                            QuestionText.text = "Q" + question_count + ". What is " + num1 + "-" + num2 + "?";
                            general_quiz = "Subtraction";
                            break;
                        case (3):
                            num1 = rnd.Next(1, 10 * diff / 2);
                            num2 = rnd.Next(1, 10 * diff / 2);
                            correctAnswer = num1 * num2;
                            QuestionText.text = "Q" + question_count + ". What is " + num1 + "x" + num2 + "?";
                            general_quiz = "Multiplication";
                            break;
                        case (4):
                            num2 = rnd.Next(1, 10 * diff / 2);
                            int tempAnswer2 = rnd.Next(1, 10 * diff / 2);
                            num1 = num2 * tempAnswer2;
                            correctAnswer = num1 / num2;
                            QuestionText.text = $"What is {num1} ÷ {num2}?";
                            general_quiz = "Division";
                            break;
                    }
                    break;

            }

            save_question(QuestionText.text, "MC");

            // Generate wrong answers
            List<int> wrongAnswers = new List<int>();
            while (wrongAnswers.Count < 3)
            {
                int wrongAnswer;
                switch (general_quiz)
                {
                    case "Addition":
                        wrongAnswer = rnd.Next(1, 10 * diff);
                        break;
                    case "Subtraction":
                        wrongAnswer = rnd.Next(1, 10 * diff);
                        break;
                    case "Multiplication":
                        wrongAnswer = rnd.Next(1, 10 * diff / 2);
                        break;
                    case "Division":
                        wrongAnswer = rnd.Next(1, 10 * diff / 2);
                        break;
                    default:
                        throw new Exception("Invalid quiz name");
                }

                if (wrongAnswer != correctAnswer && !wrongAnswers.Contains(wrongAnswer))
                {
                    wrongAnswers.Add(wrongAnswer);
                }
            }

            // Create an array of options including the correct answer
            string[] options = new string[4];
            options[0] = correctAnswer.ToString();
            for (int i = 1; i < 4; i++)
            {
                options[i] = wrongAnswers[i - 1].ToString();
            }

            // Shuffle the array
            options = options.OrderBy(x => rnd.Next()).ToArray();

            // Display the options
            float newFontSize = 60.0f;
            OptionButton1.GetComponentInChildren<TMP_Text>().text = options[0];
            OptionButton1.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
            OptionButton2.GetComponentInChildren<TMP_Text>().text = options[1];
            OptionButton2.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
            OptionButton3.GetComponentInChildren<TMP_Text>().text = options[2];
            OptionButton3.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
            OptionButton4.GetComponentInChildren<TMP_Text>().text = options[3];
            OptionButton4.GetComponentInChildren<TMP_Text>().fontSize = newFontSize;
        }

    }

    public void ReturnToMainPage(string text)
    {
        if (text == "quiz panel")
        {
            gameObject.SetActive(false);
            OptionButton1.gameObject.SetActive(false);
            OptionButton2.gameObject.SetActive(false);
            OptionButton3.gameObject.SetActive(false);
            OptionButton4.gameObject.SetActive(false);
            QuestionText.gameObject.SetActive(false);
            ScoreText.gameObject.SetActive(false);

            audioDirector.stop_director();  
            audioDirector.talk_director("Return to quiz list");
            
            quizManager.gameObject.SetActive(true);
            quizManager.ReturnButton.gameObject.SetActive(true);
        }
    }

    public void CheckAnswer(string option, string quiz_name)
    {
        Button selectedButton;

        // Get the selected button
        switch (option)
        {
            case "Option1":
                selectedButton = OptionButton1;
                break;
            case "Option2":
                selectedButton = OptionButton2;
                break;
            case "Option3":
                selectedButton = OptionButton3;
                break;
            case "Option4":
                selectedButton = OptionButton4;
                break;
            default:
                throw new Exception("Invalid option");
        }

        string selectedAnswer = selectedButton.GetComponentInChildren<TMP_Text>().text;

        bool isCorrect = false;

        // Determine if the quiz is string-based or integer-based
        if (quiz_name == "Addition" || quiz_name == "Subtraction" || quiz_name == "Multiplication" || quiz_name == "Division" || quiz_name == "Final Quiz")
        {
            // For arithmetic quizzes, parse and compare integers
            isCorrect = int.TryParse(selectedAnswer, out int selectedInt) && selectedInt == correctAnswer;
        }
        else
        {
            // For other quizzes, compare strings directly
            isCorrect = selectedAnswer.Equals(general_correctAnswer, StringComparison.OrdinalIgnoreCase);
        }

        // Check if the selected answer is correct

        //if (int.Parse(selectedAnswer) == correctAnswer)
        if (isCorrect)
        {
            score++;
            ScoreText.text = "Score: " + score;
            selectedButton.GetComponent<Image>().color = Color.green; // Change button color to green

            if (quiz_name == "Addition" || quiz_name == "Subtraction" || quiz_name == "Multiplication" || quiz_name == "Division" || quiz_name == "Final Quiz")
            {
                AudioSpeech(1, selectedAnswer, "");
            }
            else
            {
                AudioSpeech(1, general_correctAnswer, "");
            }
                
            //StartCoroutine(characterAnimation.Character_Animation("nod", "happy"));
            characterAnimation.Character_Animation("nod", "happy");

            StartCoroutine(WaitAndLoadNextQuestionMC(selectedButton, 4, quiz_name)); // Wait for 2 seconds for correct answers
        }
        else
        {
            selectedButton.GetComponent<Image>().color = Color.red; // Change button color to red

            if (quiz_name == "Addition" || quiz_name == "Subtraction" || quiz_name == "Multiplication" || quiz_name == "Division" || quiz_name == "Final Quiz")
            {
                AudioSpeech(2, correctAnswer.ToString(), "");
            }
            else
            {
                AudioSpeech(2, general_correctAnswer, "");
            }

            //StartCoroutine(characterAnimation.Character_Animation("swing", "sad"));
            characterAnimation.Character_Animation("swing", "sad");

            StartCoroutine(WaitAndLoadNextQuestionMC(selectedButton, 6, quiz_name)); // Wait for 3 seconds for incorrect answers
        }
    }

    private Coroutine aiCoroutine = null;
    public void save_question(string question_content, string type)
    {
        HintButton.onClick.RemoveAllListeners();
        HintButton.onClick.AddListener(() => Hint(question_content, type));
        HintText.GetComponentInChildren<TMP_Text>().text = "";
    }


    public void Hint(string question_content, string type)
    {
        if (aiCoroutine != null)
        {
            StopCoroutine(aiCoroutine);
            aiCoroutine = null;
        }
        HintText.gameObject.SetActive(true);
        HintText.onClick.RemoveAllListeners();
        HintText.GetComponentInChildren<TMP_Text>().text = "Loading hint...";
        Debug.Log("Asking question: " + question_content);
        string template = "";
        if (type == "MC")
        {
            template = "Can you show me a short hint with just 2 sentences of what is the answer of the question.  The question is " + question_content + " Which is a MC question. Don’t tell me the answer directly, I want to figure out the answer by myself!  Start the answer with “To solve”.";
        }
        else if (type == "Drag")
        {
            template = "Can you show me a short hint with just 2 sentences of what operator is the correct answer for the question. The question is " + question_content + " .Don’t tell me the answer directly, I want to figure out the answer by myself!  Start the answer with “To solve”.";
        }
        else if (type == "Inequality")
        {
            template = "This is question that need to drag and swap 4 numbers to make left side equal to right side.Can you show me a short hint with just 2 sentences of what is the answer of the question? The question is " + question_content + ".Don’t tell me the answer directly, I want to figure out the answer by myself!Start the answer with “To solve”.";
        }   
        // Ask AI and place the result in hint text
        aiCoroutine = StartCoroutine(aI_Algorithm.AI_responseCoroutine(template, (response) =>
        {
            string temp = response.Split("[ASSISTANT]:")[response.Split("[ASSISTANT]:").Length - 1];
            HintText.GetComponentInChildren<TMP_Text>().text = temp;
            aiCoroutine = null;
        }));

    }

    private void AudioSpeech(int type, string correctAnswer, string speech)
    {
        if (speech == "")
        {
            if (type == 1)
            {
                string[] positiveFeedback = new string[]
                {
                "Correct!",
                "Exactly!",
                "Nice!",
                "Right!",
                "Spot on!",
                "Superb!",
                "Bravo!",
                "Perfect!",
                "You nailed it!"
                };

                // Select a random feedback from the array
                System.Random random = new System.Random();
                int index = random.Next(positiveFeedback.Length);

                // Use the selected feedback
                audioDirector.talk_director(positiveFeedback[index]);
            }
            else if (type == 2)
            {
                string[] correctiveFeedback = new string[]
                 {
                "Good try. The correct answer is {0}.",
                "Not quite. The correct answer is {0}.",
                "Almost! The correct answer is {0}.",
                "Close! The correct answer is {0}.",
                "The correct answer is {0}. Keep learning!",
                "Try again. The correct answer is {0}.",
                "No worries. The correct answer is {0}.",
                "Oops! The correct answer is {0}.",
                "Tricky one. The correct answer is {0}."
                 };

                // Select a random feedback from the array
                System.Random random = new System.Random();
                int index = random.Next(correctiveFeedback.Length);

                // Use the selected feedback
                string feedback = String.Format(correctiveFeedback[index], correctAnswer);
                audioDirector.talk_director(feedback);
            }
        }    
        else
        {
            audioDirector.talk_director(speech);
        }

    }

    // Coroutine to wait and load the next question
    private IEnumerator WaitAndLoadNextQuestionMC(Button selectedButton, int waitTime, string quiz_name)
    {
        yield return new WaitForSeconds(waitTime);

        // Reset button color
        Color original_color = new Color(1f, 1f, 1f, 150f / 255f);
        selectedButton.GetComponent<Image>().color = original_color;

        currentQuestionIndex++;
        DisplayQuestion(quiz_name);
    }

    private IEnumerator WaitAndLoadNextQuestionDrop(int waitTime, string quiz_name)
    {
        yield return new WaitForSeconds(waitTime);

        currentQuestionIndex++;
        DisplayQuestion(quiz_name);
    }
}

