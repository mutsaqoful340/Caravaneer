using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class VNPanel : MonoBehaviour
{
    public VNDialogueSystem dialogueSystem;
    public Animator animator;

    public void OnPanelDisable_Left()
    {
        dialogueSystem.PrvPanelDisable_Left();
    }

        public void OnPanelDisable_Right()
    {
        dialogueSystem.PrvPanelDisable_Right();
    }
}