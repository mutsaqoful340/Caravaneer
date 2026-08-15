using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public enum PanelState
{
    Confirm,
    Cancel
}
public class UI_UnivConfirmPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;
    private Action onCancel;

    public void OnShow(
        string title,
        string message,
        Action confirmAction,
        Action cancelAction = null)
    {
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

        Close();

        action?.Invoke();
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

        gameObject.SetActive(false);
    }
}