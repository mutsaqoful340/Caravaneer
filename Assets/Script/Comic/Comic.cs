using UnityEngine;
using UnityEngine.UI;

public class Comic : MonoBehaviour
{
    public ComicPanel[] comicPanels;
    public Animator animator;
    public Image backrgoundSprite;
    private int currentPanelIndex;
    private bool hasShown;

    private void Start()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger("Show");
        OnPlayComic();
    }

    public void OnPlayComic()
    {
        if (!hasShown)
        {
            animator.SetTrigger("Show");
            hasShown = true;
        }
        
        while (comicPanels != null && currentPanelIndex < comicPanels.Length)
        {
            ComicPanel comicPanel = comicPanels[currentPanelIndex];
            currentPanelIndex++;

            if (comicPanel)
            {
                comicPanel.OnShowPanel();
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
                comicPanel.OnHidePanel();
                animator.SetTrigger("Hide");
            }
        }
    }

    public void OnComicDisable()
    {
        gameObject.SetActive(false);
    }
}