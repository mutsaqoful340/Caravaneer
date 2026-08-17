using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum PanelState
{
    Confirm,
    Cancel
}
public class UI_UnivConfirmPanel : MonoBehaviour
{
    public static UI_UnivConfirmPanel Instance { get; set; }
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;
    private Action onCancel;

    private CanvasGroup callerCanvas;
    private GameObject previousSelection;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        gameObject.SetActive(false);
    }

    public void OnShow(
        string title,
        string message,
        Action confirmAction,
        Action cancelAction = null,
        CanvasGroup callerCanvasToFreeze = null)
    {
        callerCanvas = callerCanvasToFreeze;
        previousSelection = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

        if (callerCanvas != null)
        {
            callerCanvas.interactable = false;
        }

        SelectFirstButton();
        titleText.text = title;
        messageText.text = message;

        onConfirm = confirmAction;
        onCancel = cancelAction;

        gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
    }

    private void Confirm()
    {
        Action action = onConfirm;

        gameObject.SetActive(false);   // hide panel first
        onConfirm = null;
        onCancel = null;

        action?.Invoke();              // let UseItem()/Destroy() happen now

        RestoreCallerInteraction();    // check activeInHierarchy after destruction is real
    }

    private void Cancel()
    {
        Action action = onCancel;

        Close();

        action?.Invoke();
    }

    private void Close()
    {
        onConfirm = null;
        onCancel = null;
        
        RestoreCallerInteraction();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        onConfirm = null;
        onCancel = null;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    private void SelectFirstButton()
    {
        if (confirmButton != null)
        {
            confirmButton.Select();
        }
    }

    private void RestoreCallerInteraction()
    {
        if (callerCanvas == null) return;

        // Host on Manager_UI (always active) since this panel deactivates itself right after this call.
        MonoBehaviour coroutineHost = Manager_UI.Instance != null ? Manager_UI.Instance : this;
        coroutineHost.StartCoroutine(RestoreCallerInteractionNextFrame(callerCanvas, previousSelection));

        callerCanvas = null;
        previousSelection = null;
    }

    // Waits a frame so any Destroy() triggered by the caller's action has actually taken effect.
    private IEnumerator RestoreCallerInteractionNextFrame(CanvasGroup canvas, GameObject previous)
    {
        yield return null;

        canvas.interactable = true;

        if (EventSystem.current == null) yield break;

        if (previous != null && previous.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(previous);
            Debug.Log($"Restored previous selection: {previous.name}");
        }
        else
        {
            Selectable firstSelectable = canvas.GetComponentInChildren<Selectable>(true);
            if (firstSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
                Debug.Log($"Restored first selectable in caller canvas: {firstSelectable.name}");
            }
        }
    }
}