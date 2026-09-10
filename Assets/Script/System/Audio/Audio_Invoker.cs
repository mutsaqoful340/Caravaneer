using UnityEngine;

public enum SFXType
{
    OneShot,
    Continuous
}
public class Audio_Invoker : MonoBehaviour
{
    public GameObject worldPlayerPrefab; // Will spawn the prefab at the caller's world transform
    public SFXType sFXType = SFXType.OneShot;

    public void OnPlaySFXLocal(string sfxName)
    {
        OnPlaySFXLocal(sfxName, sFXType);
    }

    public void OnPlaySFXLocal(string sfxName, SFXType newSFXType)
    {
        if (worldPlayerPrefab == null)
        {
            Debug.LogWarning("Audio_Invoker requires a world player prefab.");
            return;
        }

        if (Manager_Audio.Instance == null)
        {
            Debug.LogWarning("Cannot play SFX because Manager_Audio is missing.");
            return;
        }

        AudioClip clip = Manager_Audio.Instance.GetSFX(sfxName);
        if (clip == null)
        {
            return;
        }

        GameObject worldPlayerObject = Instantiate(
            worldPlayerPrefab,
            transform.position,
            transform.rotation);

        OnSFXType(newSFXType, worldPlayerObject, clip);
    }

    public void OnSFXType(SFXType newSFXType, GameObject worldPlayerObject, AudioClip clip)
    {
        switch (newSFXType)
        {
            case SFXType.OneShot:
                if (worldPlayerObject.TryGetComponent<Audio_Player>(out Audio_Player oneShotPlayer))
                {
                    oneShotPlayer.PlaySFX(clip);
                    return;
                }
                break;
            case SFXType.Continuous:
                if (worldPlayerObject.TryGetComponent<Audio_Player>(out Audio_Player continuePlayer))
                {
                    worldPlayerObject.transform.SetParent(transform);
                    continuePlayer.SFX.loop = true;
                    continuePlayer.PlaySFX(clip);
                    return;
                }
                break;
            default:
                break;
        }

        Debug.LogWarning($"The world player prefab is missing an audio player for SFX type '{newSFXType}'.");
        Destroy(worldPlayerObject);
    }
}