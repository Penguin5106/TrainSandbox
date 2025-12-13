using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum UIPanel
{
    Train, Tile, Rail, Station, None
}

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    [SerializeField] private GameObject TrainInfoPanel;
    [SerializeField] private GameObject TileInfoPanel;
    
    [SerializeField] private GameObject TileNameText;
    [SerializeField] private GameObject TileNameBox;
    
    [SerializeField] private GameObject TrainTimetableText;
    [SerializeField] private GameObject StationsDropdown;
    
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
            case UIPanel.Station:
            {
                TileInfoPanel.SetActive(true);
                break;
            }
            case UIPanel.Rail:
            {
                TileNameBox.GetComponent<TMP_InputField>().text = "enter name to promote rail to a station";
                TileNameText.GetComponent<TextMeshProUGUI>()?.SetText("");
                TileInfoPanel.SetActive(true);
                break;   
            }
            case UIPanel.Tile:
            {
                TileNameBox.GetComponent<TMP_InputField>().text = "enter name to promote rail to a station";
                TileNameText.GetComponent<TextMeshProUGUI>()?.SetText(""); 
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

    public void SetStationText(string StationName)
    {
        TileNameBox.GetComponent<TMP_InputField>().text = StationName;
        TileNameText.GetComponent<TextMeshProUGUI>()?.SetText(StationName);
    }

    public void SetTrainInfo(List<Station> Timetable)
    {
        TrainTimetableText.GetComponent<TextMeshProUGUI>().text = FormatTimetable(Timetable);

        TMP_Dropdown dropdown = StationsDropdown.GetComponent<TMP_Dropdown>();

        dropdown.ClearOptions();
        
        foreach (Station station in GameEngine.GetInstance().GetStations())
        {
            dropdown.options.Add(new  TMP_Dropdown.OptionData(station.GetName()));
        }
    }

    private string FormatTimetable(List<Station> Timetable)
    {
        if (Timetable.Count == 0)
            return "";
        
        string returnValue = "";

        foreach (Station station in Timetable)
        {
            returnValue += station.GetName() + "\n";
        }
        
        return returnValue;
    }

    public void AddStationToSelectedTimetable()
    {
        TMP_Dropdown dropdown = StationsDropdown.GetComponent<TMP_Dropdown>();
        InputHandler.GetInstance().AddStationByName(dropdown.options[dropdown.value].text);
    }

}
