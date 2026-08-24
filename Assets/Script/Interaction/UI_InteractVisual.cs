using UnityEngine;
using UnityEngine.UI;

public class UI_InteractVisual : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject interactIconPivot;
    public GameObject interactHintIcon;
    public GameObject interactIcon;
    public Vector3 offset = new Vector3(0, 2f, 0); // Offset to position the visual above the object

    private Image interactIconImage;

    private void Start()
    {
        interactIconImage = interactIcon.GetComponent<Image>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            interactHintIcon.SetActive(true);
            interactIcon.SetActive(false);
        }
    }

    public void SetHoldProgress(float progress)
    {
        if (interactIconImage != null)
        {
            interactIconImage.fillAmount = Mathf.Clamp01(progress);
        }
    }
    
    private void LateUpdate()
    {
        if (mainCamera != null && interactIconPivot && interactIcon != null && interactHintIcon)
        {
            Vector3 gameObjectDirection = mainCamera.transform.position - transform.position;
            gameObjectDirection.y = 0f;

            if (gameObjectDirection.sqrMagnitude > 0f)
            {
                transform.rotation = Quaternion.LookRotation(gameObjectDirection, Vector3.up);
            }

            Vector3 visualDirection = mainCamera.transform.position - interactIconPivot.transform.position;

            if (visualDirection.sqrMagnitude > 0f)
            {
                Quaternion visualFacingRotation = Quaternion.LookRotation(visualDirection, Vector3.up);
                Vector3 visualRotation = interactIconPivot.transform.eulerAngles;
                visualRotation.x = visualFacingRotation.eulerAngles.x;
                interactIconPivot.transform.eulerAngles = visualRotation;
            }
        }
    }
}
