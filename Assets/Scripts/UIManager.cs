using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameInstanceManager gameInstanceManager;
    [SerializeField] private UIDocument uiDoc;

    public List<string> retrievedNamesList;
    private Button _addPlayerBtn;
    private Button _removePlayerBtn;
    private ListView _playerListView;
    private TextField _txtInputPlayerName;
    

    // Start is called before the first frame update
    private void Awake()
    {
        Input.ResetInputAxes();
        //Root UI element
        VisualElement rootUI = uiDoc.rootVisualElement;
        
        //TextFields
        _txtInputPlayerName = rootUI.Q<TextField>("AddPlayerTxtInput");
        
        
        //Buttons
        _addPlayerBtn = rootUI.Q<Button>("AddPlayerButton");
        _addPlayerBtn.clicked += AddPlayerToList;

        _removePlayerBtn = rootUI.Q<Button>("RemovePlayerButton");
        _removePlayerBtn.clicked += RemovePlayerFromList;
        
        //Lists
        _playerListView = rootUI.Q<ListView>("PlayerListView");

        
        InitialListLoad();
    }

    private void InitialListLoad()
    {
        if(File.Exists(Path.Combine(Application.streamingAssetsPath, "PlayerList.txt")))
        {
            string[] retrieveNames = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, "PlayerList.txt"));
            retrievedNamesList = new List<string>(retrieveNames);
            _playerListView.itemsSource = retrievedNamesList;
        }
        else
        {
            File.Create(Path.Combine(Application.streamingAssetsPath, "PlayerList.txt")).Close();
            InitialListLoad();
        }
        
    }

    private void AddPlayerToList()
    {
        retrievedNamesList.Add(_txtInputPlayerName.text);
        _playerListView.RefreshItems();
        StreamWriter writer = new StreamWriter((Path.Combine(Application.streamingAssetsPath, "PlayerList.txt")));
        foreach (var s in retrievedNamesList)
        {
            writer.WriteLine(s);

        }
        writer.Close();
        
    }
    
    

    private void RemovePlayerFromList()
    {
        string removePlayerName = _playerListView.selectedItem.ToString();
        retrievedNamesList.Remove(removePlayerName);
        _playerListView.RefreshItems();
        
        StreamWriter writer = new StreamWriter((Path.Combine(Application.streamingAssetsPath, "PlayerList.txt")));
        foreach (var s in retrievedNamesList)
        {
            writer.WriteLine(s);

        }
        writer.Close();
    }
}
