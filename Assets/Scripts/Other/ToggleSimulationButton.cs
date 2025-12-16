using UnityEngine;

public class ToggleSimulationButton : MonoBehaviour, IClickable
{
    public void Click()
    {
        UIManager.GetInstance().SetUIPanelActive(UIPanel.None);
        GameEngine.GetInstance().ToggleSimulationLoop();
    }
}
