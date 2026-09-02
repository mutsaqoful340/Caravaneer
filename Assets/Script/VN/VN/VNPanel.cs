using UnityEngine;
using UnityEngine.Events;

public class VNPanel : MonoBehaviour
{
    public VNDialogueSystem dialogueSubsystem;
    public UnityEvent onPanelDisplay;

    public void OnDisablePanel()
    {
        if (dialogueSubsystem != null) onPanelDisplay.Invoke();
    }
}