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
    public Animator animator;

    // Serves as reference to the current character data being displayed
    [Header("Left Side Panel Settings")]
    public GameObject leftPanel;
    public Animator leftPanelAnimator;
    public Image leftCharacterSprite;
    public TextMeshProUGUI leftCharacterName;
    public TextMeshProUGUI leftCharacterDialogue;
    public bool isLeftSide = false; // Determines if the dialogue should be displayed on the left side

    // Serves as reference to the current character data being displayed
    [Header("Right Side Panel Settings")]
    public GameObject rightPanel;
    public Animator rightPanelAnimator;
    public Image rightCharacterSprite;
    public TextMeshProUGUI rightCharacterName;
    public TextMeshProUGUI rightCharacterDialogue;
    public bool isRightSide = false; // Determines if the dialogue should be displayed on the right side

    private void Start()
    {
        // Ensure panels are inactive at the start
        if (leftPanel != null) leftPanel.SetActive(false);
        if (rightPanel != null) rightPanel.SetActive(false);
        if (animator == null) animator = GetComponent<Animator>();
        if (leftPanelAnimator == null) leftPanelAnimator = leftPanel.GetComponent<Animator>();
        if (rightPanelAnimator == null) rightPanelAnimator = rightPanel.GetComponent<Animator>();
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
                isLeftSide = true;
                PrvPanelToggle_Left();
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
                isRightSide = true;
                PrvPanelToggle_Right();
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

        PrvPanelHide_All();
        Debug.Log("Dialogue sequence finished!");
        isDialogueActive = false;
        showDialogue = false;
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

    private void PrvPanelToggle_Right()
    {
        rightPanel.SetActive(true);
        rightPanelAnimator.SetTrigger("Show");

        if (isLeftSide)
        {
            leftPanelAnimator.SetTrigger("Hide");
            isLeftSide = false; // Reset left side flag when right side is active
        }
    }

    private void PrvPanelToggle_Left()
    {
        leftPanel.SetActive(true);
        leftPanelAnimator.SetTrigger("Show");

        if (isRightSide)
        {
            rightPanelAnimator.SetTrigger("Hide");
            isRightSide = false; // Reset right side flag when left side is active
        }
    }

    private void PrvPanelHide_All()
    {
        if (leftPanel)
        {
            leftPanelAnimator.SetTrigger("Hide");
            isLeftSide = false;
        }
        if (rightPanel)
        {
            rightPanelAnimator.SetTrigger("Hide");
            isRightSide = false;
        }

    }

    public void PrvPanelDisable_Left()
    {
        if (leftPanel)
        {
            leftPanel.SetActive(false);
            isLeftSide = false;
        }
    }

    public void PrvPanelDisable_Right()
    {
        if (rightPanel)
        {
            rightPanel.SetActive(false);
            isRightSide = false;
        }
    }
}