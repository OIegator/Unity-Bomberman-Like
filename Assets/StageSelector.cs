using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class StageSelector : MonoBehaviour
{
    public GameObject buttonPrefab; // Prefab for the stage button
    public GameObject pageContainerPrefab; // Prefab for the page container
    public RectTransform scrollContent; // Content of the scroll rect
    public int stagesPerPage = 5; // Number of stages per page
    public int totalStages = 20; // Total number of stages
    public Color selectedColor; // Color for selected button
    public Color defaultColor; // Default color for buttons

    private List<List<Button>> stageButtons = new List<List<Button>>();
    private Button selectedButton;

    void Start()
    {
        GenerateStageButtons();
        SelectStage(0); // Select the first stage by default
    }

    void GenerateStageButtons()
    {
        int totalPages = Mathf.CeilToInt((float)totalStages / stagesPerPage);

        for (int page = 0; page < totalPages; page++)
        {
            GameObject pageContainerGO = Instantiate(pageContainerPrefab, scrollContent);
            RectTransform pageContainerRect = pageContainerGO.GetComponent<RectTransform>();

            List<Button> pageButtonList = new List<Button>();

            for (int i = 0; i < stagesPerPage; i++)
            {
                int stageIndex = page * stagesPerPage + i;
                if (stageIndex >= totalStages)
                    break;

                GameObject buttonGO = Instantiate(buttonPrefab, pageContainerRect);
                Button button = buttonGO.GetComponent<Button>();
                TextMeshProUGUI  buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "" + (stageIndex + 1);

                // Set button interactable based on unlocked stages
                button.interactable = CheckIfStageUnlocked(stageIndex);

                // Add listener for button click
                int index = stageIndex; // To capture the current value of stageIndex
                button.onClick.AddListener(() => SelectStage(index));

                pageButtonList.Add(button);
            }

            stageButtons.Add(pageButtonList);
        }
    }

    bool CheckIfStageUnlocked(int stageIndex)
    {
        // Implement your logic to check if the stage is unlocked
        // For example, you can use player preferences or a data structure to track unlocked stages
        // Return true if the stage is unlocked, otherwise false
        return true; // Placeholder
    }

    void SelectStage(int stageIndex)
    {
        // Change color of previously selected button back to default
        if (selectedButton != null)
            selectedButton.GetComponent<Image>().color = defaultColor;

        // Select the new button
        selectedButton = stageButtons[stageIndex / stagesPerPage][stageIndex % stagesPerPage];
        selectedButton.GetComponent<Image>().color = selectedColor;

        // Implement your logic to handle stage selection
        Debug.Log("Selected Stage: " + (stageIndex + 1));
    }
}
