using UnityEngine;

public class Manager_Audio : MonoBehaviour
{
    public static Manager_Audio Instance {get; set;}

    public AudioClip footStep;
    public AudioClip swordSwing;
    public AudioClip enemySwordSwing;
    public AudioClip axeSwing;

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
    }

    #region Public Methods
    public AudioClip GetSFX(string sfxName)
    {
        switch (sfxName)
        {
            case "FootStep":
                return footStep;
            case "SwordSwing":
                return swordSwing;
            case "EnemySwordSwing":
                return enemySwordSwing;
            case "AxeSwing":
                return axeSwing;
            default:
                Debug.LogWarning($"Audio clip '{sfxName}' was not found.");
                return null;
        }
    }
    #endregion

    #region Helper Methods
    #endregion
}