using Azure.Core.Pipeline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;

public class DragDropQuestion : MonoBehaviour
{
    // Operators and Drop Area
    public GameObject ObjecttoDrag;
    public GameObject ObjectPlacer;

    // Submit button
    public Button SubmitButton;

    // Text Field
    public TMP_Text l_input;
    public TMP_Text r_input;

    // Quistion field
    public TMP_Text question_field;

    public float Dropdistance;

    public bool islocked;

    Vector2 ObjectPos;

    private static GameObject currentObjectInPlacer;

    public IndividualQuiz individualQuiz;

    public void DisplayDragDropQuestion(string chapter, int question_count, int diff)
    {
        // Reset the state of the dragged object and currentObjectInPlacer
        if (currentObjectInPlacer != null)
        {
            currentObjectInPlacer.GetComponent<DragDropQuestion>().islocked = false;
            currentObjectInPlacer.transform.position = currentObjectInPlacer.GetComponent<DragDropQuestion>().ObjectPos;
            currentObjectInPlacer = null;
        }

        string correct_op = QuestionGenerator(chapter, question_count, diff);

        SubmitButton.image.color = Color.white;
        SubmitButton.onClick.RemoveAllListeners(); // Remove previous listeners
        SubmitButton.onClick.AddListener(() => SubmitButton_Click(chapter, correct_op));
    }

    private void Start()
    {
        ObjectPos = ObjecttoDrag.transform.position;
        
    }

    public string QuestionGenerator(string chapter, int question_count, int diff)
    {
        question_field.text = $"Q.{question_count} Choose the correct operator";

        System.Random rnd = new System.Random();
        int temp1 = rnd.Next(1, 10) * diff;
        int temp2 = rnd.Next(1, 10) * diff;
        l_input.text = $"{temp1}";

        string return_string;

        switch (chapter)
        {
            case "Addition":
                int temp3 = temp1 + temp2;          
                r_input.text = $"{temp2} = {temp3}";
                return_string = "Addition";
                break;
            case "Subtraction":
                int temp4 = temp1 - temp2;
                r_input.text = $"{temp2} = {temp4}";
                return_string = "Subtraction";
                break;
            case "Multiplication":
                int temp5 = temp1 * temp2;
                r_input.text = $"{temp2} = {temp5}";
                return_string = "Multiplication";
                break;
            case "Division":
                int temp6 = temp1 * temp2;
                r_input.text = $"{temp2} = {temp6}";
                return_string = "Division";
                break;
            default:
                int op = rnd.Next(1, 5);
                if (op == 1)
                {
                    int temp7 = temp1 + temp2;
                    r_input.text = $"{temp2} = {temp7}";
                    return_string = "Addition";
                    break;
                }
                else if (op == 2)
                {
                    int temp8 = temp1 - temp2;
                    r_input.text = $"{temp2} = {temp8}";
                    return_string = "Subtraction";
                    break;
                }
                else if (op == 3)
                {
                    int temp9 = temp1 * temp2;
                    r_input.text = $"{temp2} = {temp9}";
                    return_string = "Multiplication";
                    break;
                }
                else if (op == 4)
                {
                    int temp10 = temp1 * temp2;
                    r_input.text = $"{temp2} = {temp10}";
                    return_string = "Division";
                    break;
                }
                else
                {
                    Debug.Log("Error in QuestionGenerator");
                    return_string = "Error in QuestionGenerator";
                    break;
                }
        }
        individualQuiz.save_question(l_input.text + "_" + r_input.text, "Drag");
        return return_string;
    }

    public void Drag()
    {
        if (!islocked)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(ObjecttoDrag.transform.position).z);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            ObjecttoDrag.transform.position = objPosition;
        }
    }

    public void Drop()
    {
        Vector3 objectWorldPos = Camera.main.ScreenToWorldPoint(ObjecttoDrag.transform.position);
        Vector3 placerWorldPos = Camera.main.ScreenToWorldPoint(ObjectPlacer.transform.position);

        float Distance = Vector3.Distance(objectWorldPos, placerWorldPos);
        Debug.Log("Distance between object and drop area: " + Distance);
        if (Distance < Dropdistance)
        {
            Debug.Log("Object dropped within drop distance.");

            // Check if there's already an object in place and it's not this object
            if (currentObjectInPlacer != null && currentObjectInPlacer != ObjecttoDrag)
            {
                Debug.Log("Replacing object in drop area. Current object: " + currentObjectInPlacer.name + ", New object: " + ObjecttoDrag.name);

                // Move the current object back to its original position
                currentObjectInPlacer.GetComponent<DragDropQuestion>().islocked = false;
                currentObjectInPlacer.transform.position = currentObjectInPlacer.GetComponent<DragDropQuestion>().ObjectPos;
                currentObjectInPlacer = null; // Set currentObjectInPlacer to null when removing an object

                Debug.Log("Removed object from drop area. Current object in placer: " + (currentObjectInPlacer != null ? currentObjectInPlacer.name : "null"));
            }

            // Place this object in the drop area
            ObjecttoDrag.transform.position = ObjectPlacer.transform.position;
            islocked = true;
            currentObjectInPlacer = ObjecttoDrag; // Update the current object in placer

            Debug.Log("Locked object into drop area. Current object in placer: " + currentObjectInPlacer.name);
        }
        else
        {
            Debug.Log("Object dropped outside drop distance.");

            ObjecttoDrag.transform.position = ObjectPos;

            // Check if the object being removed from the drop area is the current object in placer
            if (currentObjectInPlacer == ObjecttoDrag)
            {
                currentObjectInPlacer = null; // Set currentObjectInPlacer to null when the object is removed
                Debug.Log("Removed object from drop area. Current object in placer: " + (currentObjectInPlacer != null ? currentObjectInPlacer.name : "null"));
            }
        }
    }

    // Check does the correct operator in the 
    public void SubmitButton_Click(string chapter, string correct_op)
    {
        string current_obj = "";

        // Check if there is an object in the drop area
        if (currentObjectInPlacer != null)
        {
            current_obj = currentObjectInPlacer.name;
        }

        Debug.Log("correct_op: " + correct_op);
        Debug.Log("current_obj: " + current_obj);

        bool isCorrect = current_obj == correct_op;

        // Change the color based on correctness
        SubmitButton.GetComponent<Image>().color = isCorrect ? Color.green : Color.red;

        // Call the method on IndividualQuiz with the result
        individualQuiz.DragDropOnUserAnswered(chapter, isCorrect);
    }
}
