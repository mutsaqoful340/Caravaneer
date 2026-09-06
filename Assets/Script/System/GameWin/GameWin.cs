using UnityEngine;

public class GameWin : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            OnGameWin("GameWin");
        }
    }

    public void OnGameWin(string panelName)
    {
        Manager_UI.Instance.OnShowPanel(panelName);
        Manager_Game.Instance.SetState(GameState.UI);
    }
}