using UnityEngine;

public enum UIPanel
{
    Train, Tile, Rail, Station, None
}

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    [SerializeField] private GameObject TrainInfoPanel;
    [SerializeField] private GameObject TileInfoPanel;

    
    private GameObject[] Panels;

    public void Start()
    {
        Panels = new GameObject[] {TrainInfoPanel, TileInfoPanel };
        HideAllPanels();

        _instance = this;
    }

    public static UIManager GetInstance()
    {
        return _instance??new UIManager();
    }

    public void HideAllPanels()
    {
        foreach (var panel in Panels)
        {
            panel.SetActive(false);
        }
    }
    
    public void SetUIPanelActive(UIPanel panel)
    {
        HideAllPanels();
        
        switch (panel)
        {
            case UIPanel.Tile:
            {
                    TileInfoPanel.SetActive(true);
                break;
            }
            case UIPanel.Train:
            {
                TrainInfoPanel.SetActive(true);
                break;
            }
            case UIPanel.None:
            default:
            {
                break;
            }
        }
    }

}
