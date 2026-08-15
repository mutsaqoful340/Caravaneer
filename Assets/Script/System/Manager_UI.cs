using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class PanelReference
{
    public string name;
    public GameObject gameObject;
}

public class Manager_UI : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Panels to manage by name and GameObject reference.")]
    public PanelReference[] panels;

    private GameObject currentActivePanel;

    // Show a specific panel by index, and then get the Animator component of the panel to play the animation.
    public void OnShowPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panels.Length)
        {
            Debug.LogError("Invalid panel index: " + panelIndex);
            return;
        }

        GameObject panel = panels[panelIndex].gameObject;

        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }

        currentActivePanel = panel;
        currentActivePanel.SetActive(true);

        Button firstButton = null;
        Button[] buttons = currentActivePanel.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (button != null && button.gameObject.activeInHierarchy && button.enabled && button.IsInteractable())
            {
                firstButton = button;
                break;
            }
        }

        if (firstButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }

        // Animator animator = currentActivePanel.GetComponent<Animator>();
        // if (animator != null)
        // {
        //     animator.Play("Open");
        // }
    }

    public void OnCloseCurrentPanel()
    {
        currentActivePanel?.SetActive(false);
        currentActivePanel = null;
    }
}