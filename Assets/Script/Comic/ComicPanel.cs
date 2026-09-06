using UnityEngine;

public class ComicPanel : MonoBehaviour
{
    public Comic comicManager;
    public Animator animator;
    public bool hasShown;

    private void Start()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void OnPlayAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void OnComicAnimationComplete()
    {
        if (hasShown) return;

        hasShown = true;
        if (comicManager)
        {
            comicManager.OnPlayComic();
        }
    }
}