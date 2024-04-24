using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
using JetBrains.Annotations;
using System.Xml;
using Azure.AI.OpenAI;

public class PointsPanel : MonoBehaviour
{
    // For return to main page
    public Button ReturnButton;

    // Confirmation Panel
    public GameObject ConfirmationPanel;
    public TMP_Text ConfirmText;
    public TMP_Text Points_Confirm;
    public Button YesButton;
    public Button NoButton;


    public GameManager gameManager;

   // Derivatives
    public PointsManager pointsManager;
    public BackgroundManager backgroundManager;
    public DatabaseManager databaseManager;

    public AzureTTS audioController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function called from GameManager
    public void points_panel()
    {
        ReturnButton.onClick.RemoveAllListeners();
        ReturnButton.onClick.AddListener(ReturnToMainPage);
        gameObject.SetActive(true);
        ConfirmationPanel.SetActive(false);

    }


    // Confirmation Panel
    public void confirmation(string types, string Obj_name, int required_pts)
    {
        NoButton.onClick.RemoveAllListeners();  
        NoButton.onClick.AddListener(CloseConfirmation);

        YesButton.onClick.RemoveAllListeners(); 
        ConfirmationPanel.SetActive(true);

        // Check enough pts or not
        int temp = databaseManager.GetPoints();
        
        if (temp < required_pts)
        {
            Points_Confirm.text = "Not enough points";
            Points_Confirm.color = Color.red;
        }
        else
        {
            Points_Confirm.color = Color.white;
            Points_Confirm.text = "Remaining points: " + (temp - required_pts);
            YesButton.onClick.AddListener(() => BuyProducts(types, Obj_name));
        }
    }

    // Buy Products
    public void BuyProducts(string types, string Obj_name)
    {
        audioController.StopAudio();
        StartCoroutine(audioController.ConvertTextToSpeech("You have successfully bought a " + types));
        if (types == "chapter")
        {
            pointsManager.refresh_course_list(Obj_name);
        }
        if (types == "background")
        {
            backgroundManager.after_buying_background(Obj_name);
        }
        ConfirmationPanel.SetActive(false);
    }

    // Click no to return to points panel
    private void CloseConfirmation()
    {
        ConfirmationPanel.SetActive(false);
    }

    // Click empty area to return to main page
    public void ReturnToMainPage()
    {
        gameObject.SetActive(false);
    }
}
