using UnityEngine;

public class VNPlayer : MonoBehaviour
{
    public static VNPlayer Instance {get; set;}
    public GameObject VN_OP_Prefab;
    public GameObject VN_ED_Prefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (VN_OP_Prefab)
        {
            Instantiate(VN_OP_Prefab, transform);
        }
    }

    public void OnPlayVN_ED()
    {
        if (VN_ED_Prefab)
        {
            Instantiate(VN_ED_Prefab, transform);
        }
    }
}