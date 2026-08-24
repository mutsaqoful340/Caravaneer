using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class VNCharacterDialogue
{
    public PopupData characterData;
    public string dialogueText;
    public VNDisplayMode displayMode = VNDisplayMode.LeftSide;
    public float dialogueDelay = 0.5f; // Delay before the dialogue is displayed
    public float dialogueDuration = 2f; // Duration for which the dialogue is displayed
}

public enum VNDisplayMode
{
    LeftSide,
    RightSide
}

public class VNDialogueSystem : MonoBehaviour
{
    public static VNDialogueSystem Instance { get; set; }
    
    public VNCharacterDialogue[] characterDatas;
    public bool isDialogueActive = false;
    public bool showDialogue = false;

    // Serves as reference to the current character data being displayed
    [Header("Left Side Panel Settings")]
    public Canvas leftPanel;
    public Animator leftPanelAnimator;
    public Image leftCharacterSprite;
    public TextMeshProUGUI leftCharacterName;
    public TextMeshProUGUI leftCharacterDialogue;
    public bool isLeftSide = false; // Determines if the dialogue should be displayed on the left side

    // Serves as reference to the current character data being displayed
    [Header("Right Side Panel Settings")]
    public Canvas rightPanel;
    public Animator rightPanelAnimator;
    public Image rightCharacterSprite;
    public TextMeshProUGUI rightCharacterName;
    public TextMeshProUGUI rightCharacterDialogue;
    public bool isRightSide = false; // Determines if the dialogue should be displayed on the right side

    private void Start()
    {
        // Ensure panels are inactive at the start
        if (leftPanel != null) leftPanel.gameObject.SetActive(false);
        if (rightPanel != null) rightPanel.gameObject.SetActive(false);
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
        VNDisplayMode? lastMode = null; // Tracks the previously shown panel to detect consecutive same-side dialogue
        bool hasLeftAppeared = false;
        bool hasRightAppeared = false;

        foreach (VNCharacterDialogue characterData in characterDatas)
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

            bool isSamePanelAsLast = lastMode.HasValue && lastMode.Value == characterData.displayMode;

            if (characterData.displayMode == VNDisplayMode.LeftSide)
            {
                isLeftSide = true;
                if (!isSamePanelAsLast)
                {
                    leftPanel.sortingOrder = 4;
                    rightPanel.sortingOrder = 3;

                    if (lastMode == VNDisplayMode.RightSide)
                    {
                        isRightSide = false;
                        rightPanelAnimator.SetTrigger("Hide");
                    }

                    OnPanelShow_Left(!hasLeftAppeared);
                    hasLeftAppeared = true;
                }

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
            else if (characterData.displayMode == VNDisplayMode.RightSide)
            {
                isRightSide = true;
                if (!isSamePanelAsLast)
                {
                    rightPanel.sortingOrder = 4;
                    leftPanel.sortingOrder = 3;

                    if (lastMode == VNDisplayMode.LeftSide)
                    {
                        isLeftSide = false;
                        leftPanelAnimator.SetTrigger("Hide");
                    }

                    OnPanelShow_Right(!hasRightAppeared);
                    hasRightAppeared = true;
                }

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

            lastMode = characterData.displayMode;

            // Wait for a short duration before displaying the next dialogue
            yield return new WaitForSeconds(characterData.dialogueDuration);
        }

        OnPanelHide_All();
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

    private void OnPanelShow_Right(bool isFirstAppearance = false)
    {
        rightPanel.gameObject.SetActive(true);
        rightPanelAnimator.SetTrigger(isFirstAppearance ? "Enable" : "Show");
    }

    private void OnPanelShow_Left(bool isFirstAppearance = false)
    {
        leftPanel.gameObject.SetActive(true);
        leftPanelAnimator.SetTrigger(isFirstAppearance ? "Enable" : "Show");
    }

    public void OnPanelDisable_Left()
    {
        if (leftPanel)
        {
            leftPanel.gameObject.SetActive(false);
            isLeftSide = false;
        }
    }

    public void OnPanelDisable_Right()
    {
        if (rightPanel)
        {
            rightPanel.gameObject.SetActive(false);
            isRightSide = false;
        }
    }

    public void OnPanelHide_All()
    {
        if (leftPanel)
        {
            leftPanelAnimator.SetTrigger("Disable");
            isLeftSide = false;
        }
        if (rightPanel)
        {
            rightPanelAnimator.SetTrigger("Disable");
            isRightSide = false;
        }
    }
}