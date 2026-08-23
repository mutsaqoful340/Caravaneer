using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class PopupCharacterDialogue
{
    public PopupData characterData;
    public string dialogueText;
    public PopupDisplayMode displayMode = PopupDisplayMode.LeftSide;
    public float dialogueDelay = 0.5f; // Delay before the dialogue is displayed
    public float dialogueDuration = 2f; // Duration for which the dialogue is displayed
}

public enum PopupDisplayMode
{
    LeftSide,
    RightSide
}

public class PopupDialogueSystem : MonoBehaviour
{
    public PopupCharacterDialogue[] characterDatas;
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
        foreach (PopupCharacterDialogue characterData in characterDatas)
        {
            Debug.Log($"Displaying dialogue {dialogueIndex} of {characterDatas.Length}");
            dialogueIndex++;
            
            // Null check for character data
            if (characterData == null || characterData.characterData == null)
            {
                Debug.LogWarning("Character data or VNData is null, skipping dialogue entry!");
                continue;
            }
               
               yield return new WaitForSeconds(characterData.dialogueDelay);

            if (characterData.displayMode == PopupDisplayMode.LeftSide)
            {
                isLeftSide = true;
                PrvPanelShow_Left();
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
            else if (characterData.displayMode == PopupDisplayMode.RightSide)
            {
                isRightSide = true;
                PrvPanelShow_Right();
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

            if (characterData.displayMode == PopupDisplayMode.LeftSide)
            {
                isLeftSide = false;
                leftPanelAnimator.SetTrigger("Hide");
            }
            else if (characterData.displayMode == PopupDisplayMode.RightSide)
            {
                isRightSide = false;
                rightPanelAnimator.SetTrigger("Hide");
            }
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
    }

    private void PrvPanelShow_Right()
    {
        rightPanel.SetActive(true);
        rightPanelAnimator.SetTrigger("Show");
    }

    private void PrvPanelShow_Left()
    {
        leftPanel.SetActive(true);
        leftPanelAnimator.SetTrigger("Show");
    }

    public void PrvPanelHide_Left()
    {
        if (leftPanel)
        {
            leftPanel.SetActive(false);
            isLeftSide = false;
        }
    }

    public void PrvPanelHide_Right()
    {
        if (rightPanel)
        {
            rightPanel.SetActive(false);
            isRightSide = false;
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
}