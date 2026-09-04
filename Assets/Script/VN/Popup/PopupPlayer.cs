using UnityEngine;

public class PopupPlayer : MonoBehaviour
{
    public static PopupPlayer Instance {set; get;}
    public GameObject popup_OP_Prefab;
    public GameObject popup_ED_Prefab;

    private void Awake()
    {
        Instance = this;
    }
    
    public void OnPlayVN_OP()
    {
        if (popup_OP_Prefab)
        {
            Instantiate(popup_OP_Prefab, transform);
        }
    }

    public void OnPlayVN_ED()
    {
        if (popup_ED_Prefab)
        {
            Instantiate(popup_ED_Prefab, transform);
        }
    }
}
