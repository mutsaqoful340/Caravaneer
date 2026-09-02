using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[System.Serializable]
public class PanelReference
{
    public string name;
    public GameObject gameObject;
    public bool hasAnimation;
}

public class Manager_UI : MonoBehaviour
{
    public static Manager_UI Instance { get; private set; }
    [Header("UI Panels")]
    [Tooltip("Panels to manage by name and GameObject reference.")]
    public PanelReference[] panels;

    private GameObject currentActivePanel;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // disables every panels at start
        foreach (PanelReference panelReference in panels)
        {
            panelReference.gameObject.SetActive(false);
        }
    }

    // Show a specific panel by name.
    public void OnShowPanel(string panelName)
    {
        Debug.Log($"Attempting to show panel: {panelName}");
        PanelReference selectedPanel = null;

        foreach (PanelReference panelReference in panels)
        {
            if (panelReference != null && panelReference.name == panelName)
            {
                selectedPanel = panelReference;
                break;
            }
        }

        if (selectedPanel == null || selectedPanel.gameObject == null)
        {
            Debug.LogError("No panel found with name: " + panelName);
            return;
        }

        GameObject panel = selectedPanel.gameObject;

        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }

        currentActivePanel = panel;
        Animator animator = currentActivePanel.GetComponent<Animator>();
        if (animator != null && selectedPanel.hasAnimation)
        {
            currentActivePanel.SetActive(true);
            animator?.SetTrigger("Open");
        }
        else
        {
            currentActivePanel.SetActive(true);
        }

        SelectFirstButtonInPanel(currentActivePanel);

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

    public void OnCloseAllPanels()
    {
        foreach (PanelReference panelReference in panels)
        {
            if (panelReference == null || panelReference.gameObject == null)
            {
                continue;
            }

            GameObject panel = panelReference.gameObject;
            Animator animator = panel.GetComponent<Animator>();
            // UI_Panel uI_Panel = panel.GetComponent<UI_Panel>();

            if (animator != null && panelReference.hasAnimation)
            {
                animator.ResetTrigger("Open");
                animator.SetTrigger("Close"); Debug.Log($"Closing panel with animation: {panelReference.name}");
                ClearSelectedButton();
            }
            else
            {
                panel.SetActive(false);
                ClearSelectedButton();
            }
        }

        currentActivePanel = null;
    }

    // === Specific Panel Methods ===

    public void SelectFirstButtonInPanel(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }

        Button[] buttons = panel.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (button != null && button.gameObject.activeInHierarchy && button.enabled && button.IsInteractable())
            {
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                }

                return;
            }
        }
    }

    private void ClearSelectedButton()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}