using UnityEngine;

public class SingleFrameButton : MonoBehaviour , IClickable
{
    public void Click()
    {
        UIManager.GetInstance().SetUIPanelActive(UIPanel.None);
        GameEngine.GetInstance().SimulateOneTurn();
    }
}
