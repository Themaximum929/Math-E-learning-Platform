using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InequalityDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Transform objectToSwapWith = null;
    private Canvas canvas;

    private bool isSwapping = false;

    public TMP_Text[] textBoxes;
    public TMP_Text[] operators;
    public Button submitButton;

    public TMP_Text inequality_text;

    public IndividualQuiz individualQuiz;

    string general_quiz_name;
    private string answer = "";

    private void Awake()
    {
        // Get the Canvas component when the object is initialized
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Cannot find Canvas component in parent GameObjects.");
        }

    }

    public void DisplayInequalityQuestion(string quiz_name, int question_count, int diff)
    {
        inequality_text.text = "Q" + question_count + ". Swap numbers to solve inequality";

        ResetSubmitButton();

        // Ensure the GameObject is active before trying to find a Canvas
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("GameObject is not active in hierarchy, cannot find Canvas");
            return;
        }

        // Attempt to find the Canvas component in the parent hierarchy
        canvas = GetComponentInParent<Canvas>();

        // Check if the Canvas was successfully found
        if (canvas == null)
        {
            Debug.LogError("Cannot find Canvas component in parent GameObjects.");
            return;
        }

        general_quiz_name = quiz_name;
        GenerateQuestion(quiz_name, diff);

        submitButton.onClick.AddListener(() => ValidateAnswer(quiz_name));
    }

    public void GenerateQuestion(string quiz_name, int diff)
    {
        answer = "";

        System.Random rnd = new System.Random();
        int number1 = rnd.Next(1, 10 * diff);
        int number2 = rnd.Next(1, 10 * diff);
        int number3 = rnd.Next(1, 10 * diff);
        int correctAnswer = 0;
        switch (quiz_name)
        {
            case "Addition":
                correctAnswer = number1 + number2 + number3;
                operators[0].text = "+";
                operators[1].text = "+";
                break;
            case "Subtraction":
                correctAnswer = number1 - number2 - number3;
                operators[0].text = "-";
                operators[1].text = "-";
                break;
            case "Multiplication":
                number1 = rnd.Next(1, 10 * diff / 2);
                number2 = rnd.Next(1, 10 * diff / 2);
                number3 = rnd.Next(1, 10 * diff / 2);
                correctAnswer = number1 * number2 * number3;
                operators[0].text = "×";
                operators[1].text = "×";
                break;
            case "Division":
                number2 = rnd.Next(1, 5 * diff / 2); // Ensure number2 is not too large to avoid decimal results
                number3 = rnd.Next(1, 5 * diff / 2);
                correctAnswer = number1 / number2 / number3;
                number1 = correctAnswer * number2 * number3; // Adjust number1 to ensure integer division
                operators[0].text = "÷";
                operators[1].text = "÷";
                break;
            default:
                int random = rnd.Next(0, 4);
                switch (random)
                {
                    case 0:
                        correctAnswer = number1 + number2 + number3;
                        operators[0].text = "+";
                        operators[1].text = "+";
                        general_quiz_name = "Addition";
                        break;
                    case 1:
                        correctAnswer = number1 - number2 - number3;
                        operators[0].text = "-";
                        operators[1].text = "-";
                        general_quiz_name = "Subtraction";
                        break;
                    case 2:
                        correctAnswer = number1 * number2 * number3;
                        operators[0].text = "×";
                        operators[1].text = "×";
                        general_quiz_name = "Multiplication";
                        break;
                    case 3:
                        number2 = rnd.Next(1, 5 * diff / 2); 
                        number3 = rnd.Next(1, 5 * diff / 2);
                        correctAnswer = number1 / number2 / number3;
                        number1 = correctAnswer * number2 * number3; 
                        operators[0].text = "÷";
                        operators[1].text = "÷";
                        general_quiz_name = "Division";
                        break;
                }
                break;
        }

        answer = number1 + " " + operators[0].text + " "+ number2 + " " + operators[1].text + " " + number3 + " = " + correctAnswer;
        

        List<int> numbers = new List<int> { number1, number2, number3, correctAnswer };
        FisherYatesShuffle(numbers);

        for (int i = 0; i < textBoxes.Length; i++)
        {
            if (i < numbers.Count)
            {
                textBoxes[i].text = numbers[i].ToString();
            }
        }

        string send_temp = textBoxes[0].text + " " + operators[0].text + " " + textBoxes[1].text + " " + operators[1].text + " " + textBoxes[2].text + " = " + textBoxes[3].text;
        individualQuiz.save_question(send_temp, "Inequality");
    }
    private void ResetSubmitButton()
    {
        submitButton.image.color = Color.white;
        submitButton.onClick.RemoveAllListeners();
    }

    private void FisherYatesShuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random(); // Use System.Random for shuffling
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void ValidateAnswer(string quiz_name)
    {
        bool isCorrect = false;

        List<TMP_Text> sortedTextboxes = new List<TMP_Text>(textBoxes);
        sortedTextboxes.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        // Calculate the result based on the quiz type
        int calculatedResult = 0;
        Debug.Log(general_quiz_name);
        switch (general_quiz_name)
        {
            case "Addition":
                for (int i = 0; i < sortedTextboxes.Count - 1; i++)
                {
                    int number;
                    if (int.TryParse(sortedTextboxes[i].text, out number))
                    {
                        calculatedResult += number;
                    }
                    else
                    {
                        Debug.LogError("One of the draggable numbers is not a valid integer");
                        return;
                    }
                }
                break;
            case "Subtraction":
                calculatedResult = int.Parse(sortedTextboxes[0].text);
                for (int i = 1; i < sortedTextboxes.Count - 1; i++)
                {
                    int number;
                    if (int.TryParse(sortedTextboxes[i].text, out number))
                    {
                        calculatedResult -= number;
                    }
                    else
                    {
                        Debug.LogError("One of the draggable numbers is not a valid integer");
                        return;
                    }
                }
                break;
            case "Multiplication":
                calculatedResult = 1;
                for (int i = 0; i < sortedTextboxes.Count - 1; i++)
                {
                    int number;
                    if (int.TryParse(sortedTextboxes[i].text, out number))
                    {
                        calculatedResult *= number;
                    }
                    else
                    {
                        Debug.LogError("One of the draggable numbers is not a valid integer");
                        return;
                    }
                }
                break;
            case "Division":
                calculatedResult = int.Parse(sortedTextboxes[0].text);
                for (int i = 1; i < sortedTextboxes.Count - 1; i++)
                {
                    int number;
                    if (int.TryParse(sortedTextboxes[i].text, out number))
                    {
                        if (number == 0)
                        {
                            Debug.LogError("Division by zero is not allowed");
                            return;
                        }
                        calculatedResult /= number;
                    }
                    else
                    {
                        Debug.LogError("One of the draggable numbers is not a valid integer");
                        return;
                    }
                }
                break;
            default:
                Debug.LogError("Invalid quiz name: " + quiz_name);
                return;
        }

        // Get the last text box (result value)
        TMP_Text resultTextbox = sortedTextboxes[sortedTextboxes.Count - 1];
        int resultValue;
        if (int.TryParse(resultTextbox.text, out resultValue))
        {
            // Check if the calculated result matches the result value
            isCorrect = calculatedResult == resultValue;
        }
        else
        {
            Debug.LogError("The draggable result value is not a valid integer");
            return;
        }

        // Change the button color based on whether the answer is correct
        submitButton.image.color = isCorrect ? Color.green : Color.red;

        if (isCorrect)
        {
            answer = "";
        }
        else
        {
            answer = "Not quite.  The answer can be: " + answer;
        }

        // Return to the individual quiz with the result
        individualQuiz.DragDropOnUserAnswered(quiz_name, isCorrect, answer);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = this.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return; // Guard clause to prevent NullReferenceExceptionA

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos
        );
        transform.position = canvas.transform.TransformPoint(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSwapping && objectToSwapWith != null)
        {
            // Get the index of the current object and the object to swap with in the textBoxes array
            int index1 = System.Array.IndexOf(textBoxes, this.GetComponent<TMP_Text>());
            int index2 = System.Array.IndexOf(textBoxes, objectToSwapWith.GetComponent<TMP_Text>());

            // Check if both objects are part of the textBoxes array
            if (index1 != -1 && index2 != -1)
            {
                // Swap the positions of the two objects
                Vector3 positionToSwapWith = objectToSwapWith.position;
                objectToSwapWith.position = originalPosition;
                this.transform.position = positionToSwapWith;

                Debug.Log(textBoxes[0].text + " " + textBoxes[1].text + " " + textBoxes[2].text + " " + textBoxes[3].text);
            }
            else
            {
                Debug.LogError("Failed to swap textBoxes values: One of the objects is not in the textBoxes array.");
            }

            objectToSwapWith = null; // Reset the object to swap with
            isSwapping = false; // Reset the swapping flag
        }
        else
        {
            // If we didn't drop on another box, reset to the original position
            this.transform.position = originalPosition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TMPInputField") && other.transform != this.transform && other.GetComponent<InequalityDrag>() != null)
        {
            objectToSwapWith = other.transform;
            isSwapping = true; // Set the flag to indicate a swap should happen
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == objectToSwapWith)
        {
            objectToSwapWith = null;
            isSwapping = false; // Reset the swapping flag
        }
    }
}