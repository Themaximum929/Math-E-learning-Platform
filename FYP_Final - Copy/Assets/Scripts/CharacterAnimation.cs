using Live2D.Cubism.Framework.Expression;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CharacterAnimation : MonoBehaviour
{
    private Animator charanimator;
    private Live2D.Cubism.Framework.Expression.CubismExpressionController expressioncontroller;
    // Start is called before the first frame update
    void Start()
    {
        charanimator = GetComponent<Animator>();
        expressioncontroller = GetComponent<Live2D.Cubism.Framework.Expression.CubismExpressionController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Character_Motion();
        //Facial_Expression();

    }

    void Character_Motion()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            charanimator.SetTrigger("nodTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            charanimator.SetTrigger("swingTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            charanimator.SetTrigger("ponTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            charanimator.SetTrigger("special1Trigger");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            charanimator.SetTrigger("special2Trigger");
        }
    }

    void Facial_Expression()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            expressioncontroller.CurrentExpressionIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            expressioncontroller.CurrentExpressionIndex = 1;
        }
    }

    public void Character_Animation(string motion, string expression)
    { 
        switch(motion)
        {
            case "default":
                charanimator.SetTrigger("initialTrigger");
                break;
            case "nod":
                charanimator.SetTrigger("nodTrigger");
                break;
            case "swing":
                charanimator.SetTrigger("swingTrigger");
                break;
            case "pon":
                charanimator.SetTrigger("ponTrigger");
                break;
            case "special1":
                charanimator.SetTrigger("special1Trigger");
                break;
            case "special2":
                charanimator.SetTrigger("special2Trigger");
                break;
            case "special3":
                charanimator.SetTrigger("special3Trigger");
                break;
            default:
                break;
        }
        switch(expression)
        {
            case "default":
                expressioncontroller.CurrentExpressionIndex = 0;
                break; 
            case "happy":
                expressioncontroller.CurrentExpressionIndex = 1;
                break;
            case "relief":
                expressioncontroller.CurrentExpressionIndex = 2;
                break;
            case "kirara":
                expressioncontroller.CurrentExpressionIndex = 3;
                break;
            case "sad":
                expressioncontroller.CurrentExpressionIndex = 4;
                break;
            case "shy":
                expressioncontroller.CurrentExpressionIndex = 5;
                break;
            case "shocked":
                expressioncontroller.CurrentExpressionIndex = 6;
                break;
            case "dissatisfied":
                expressioncontroller.CurrentExpressionIndex = 7;
                break;
            default:
                break;
        }   
    }
}