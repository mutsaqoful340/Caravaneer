using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class CharacterDialogue
{
    public VNData characterData;
    public string dialogueText;
    public DisplayMode displayMode = DisplayMode.LeftSide;
    public float dialogueDuration = 2f; // Duration for which the dialogue is displayed
}

public enum DisplayMode
{
    LeftSide,
    RightSide
}

public class VNDialogueSystem : MonoBehaviour
{
    public CharacterDialogue[] characterDatas;
    public bool isDialogueActive = false;
    public bool showDialogue = false;

    // Serves as reference to the current character data being displayed
    [Header("Left Side Panel Settings")]
    public GameObject leftPanel;
    public Image leftCharacterSprite;
    public TextMeshProUGUI leftCharacterName;
    public TextMeshProUGUI leftCharacterDialogue;

    // Serves as reference to the current character data being displayed
    [Header("Right Side Panel Settings")]
    public GameObject rightPanel;
    public Image rightCharacterSprite;
    public TextMeshProUGUI rightCharacterName;
    public TextMeshProUGUI rightCharacterDialogue;

    private void Start()
    {
        // Ensure panels are inactive at the start
        if (leftPanel != null) leftPanel.SetActive(false);
        if (rightPanel != null) rightPanel.SetActive(false);
    }

    public void OnPlayDialogue()
    {
        if (characterDatas == null || characterDatas.Length == 0)
        {
            Debug.LogWarning("No character data available for dialogue!");
            return;
        }
        if (isDialogueActive)
        {
            Debug.LogWarning("Dialogue is already active!");
            return;
        }
        isDialogueActive = true;
        StartCoroutine(DisplayDialogue());
    }

    private IEnumerator DisplayDialogue()
    {
        int dialogueIndex = 0;
        foreach (CharacterDialogue characterData in characterDatas)
        {
            Debug.Log($"Displaying dialogue {dialogueIndex} of {characterDatas.Length}");
            dialogueIndex++;
            
            // Null check for character data
            if (characterData == null || characterData.characterData == null)
            {
                Debug.LogWarning("Character data or VNData is null, skipping dialogue entry!");
                continue;
            }

            if (characterData.displayMode == DisplayMode.LeftSide)
            {
                rightPanel.SetActive(false);
                leftPanel.SetActive(true);
                if (leftCharacterSprite != null && leftCharacterName != null && leftCharacterDialogue != null)
                {
                    leftCharacterSprite.sprite = characterData.characterData.characterSprite;
                    leftCharacterSprite.SetNativeSize();
                    leftCharacterName.text = characterData.characterData.characterName;
                    leftCharacterDialogue.text = characterData.dialogueText;
                }
                else
                {
                    Debug.LogWarning("Left panel UI elements are not assigned!");
                }
            }
            else if (characterData.displayMode == DisplayMode.RightSide)
            {
                leftPanel.SetActive(false);
                rightPanel.SetActive(true);
                if (rightCharacterSprite != null && rightCharacterName != null && rightCharacterDialogue != null)
                {
                    rightCharacterSprite.sprite = characterData.characterData.characterSprite;
                    rightCharacterSprite.SetNativeSize();
                    rightCharacterName.text = characterData.characterData.characterName;
                    rightCharacterDialogue.text = characterData.dialogueText;
                }
                else
                {
                    Debug.LogWarning("Right panel UI elements are not assigned!");
                }
            }

            // Wait for a short duration before displaying the next dialogue
            yield return new WaitForSeconds(characterData.dialogueDuration);
        }

        Debug.Log("Dialogue sequence finished!");
        isDialogueActive = false;
        showDialogue = false;
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
    }

    private void Update()
    {
        if (showDialogue && !isDialogueActive)
        {
            showDialogue = false;
            OnPlayDialogue();
        }

        // // Press Spacebar to advance or restart dialogue
        // if (isDialogueActive)
        // {
        //     StopAllCoroutines();
        //     StartCoroutine(DisplayDialogue());
        // }
    }
}