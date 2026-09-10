using UnityEngine;
using System.Collections;

/// Summary
/// One-shot play audio
public class Audio_Player : MonoBehaviour
{
    public AudioSource SFX;
    public bool randomizePitch;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public bool randomizeVolume;
    public float minVolume = 0.85f;
    public float maxVolume = 1f;

    public void PlaySFX(AudioClip clip)
    {
        if (SFX == null)
        {
            Debug.LogWarning("Audio_WorldPlayer requires an AudioSource.");
            Destroy(gameObject);
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("Audio_WorldPlayer received a null AudioClip.");
            Destroy(gameObject);
            return;
        }

        OnRandomPitch();
        OnRandomVolume();
        SFX.PlayOneShot(clip);
        StartCoroutine(EnumDestroy(clip.length));
    }

    private void OnRandomPitch()
    {
        if (randomizePitch)
        {
            SFX.pitch = Random.Range(minPitch, maxPitch);
            return;
        }

        SFX.pitch = 1f;
    }

    private void OnRandomVolume()
    {
        if (randomizeVolume)
        {
            SFX.volume = Random.Range(minVolume, maxVolume);
            return;
        }

        SFX.volume = 1f;
    }

    private IEnumerator EnumDestroy(float destroyDelay)
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}