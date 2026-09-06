using UnityEngine;

public class Comic : MonoBehaviour
{
    public ComicPanel[] comicPanels;
    public Animator animator;
    private int currentPanelIndex;

    private void Start()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger("Play");
    }

    public void OnPlayComic()
    {
        while (comicPanels != null && currentPanelIndex < comicPanels.Length)
        {
            ComicPanel comicPanel = comicPanels[currentPanelIndex];
            currentPanelIndex++;

            if (comicPanel)
            {
                comicPanel.OnPlayAnimation("Show");
                return;
            }
        }
    }

    public void OnComicOver()
    {
        foreach (ComicPanel comicPanel in comicPanels)
        {
            if (comicPanel)
            {
                comicPanel.OnPlayAnimation("Hide");
            }
        }
    }
}