using UnityEngine;
using System.Collections;

public class Audio_ContinuePlayer : MonoBehaviour
{
    public AudioSource SFX;

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

        SFX.PlayOneShot(clip);
        StartCoroutine(EnumDestroy(clip.length));
    }

    private IEnumerator EnumDestroy(float destroyDelay)
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
