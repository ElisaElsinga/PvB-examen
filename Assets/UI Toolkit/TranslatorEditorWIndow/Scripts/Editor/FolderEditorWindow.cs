using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.WSA;


public class FolderEditorWindow : EditorWindow
{
    private ScrollView scrollView;
    public ScrollView SettingScroll;
    private ScrollView SnippetScroll;
    private Button addButton;
    private List<FolderCustomControl> folderControlsList = new List<FolderCustomControl>();

    [MenuItem("Tools/FolderSelectWindow")]
    public static void OpenFolderWindow()
    {
        FolderEditorWindow wnd = GetWindow<FolderEditorWindow>();
        wnd.titleContent = new GUIContent("Folder Select Window");
        wnd.maxSize = new Vector2(2000, 750);
        wnd.minSize = wnd.maxSize;
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/UI Toolkit/TranslatorEditorWIndow/Resources/UI Documents/FolderEditorWindow.uxml");
        VisualElement tree = visualTree.Instantiate();
        root.Add(tree);

        scrollView = root.Q<ScrollView>("ScrollView");
        SettingScroll = root.Q<ScrollView>("SettingScroll");
        SnippetScroll = root.Q<ScrollView>("SnippetScroll");

        addButton = root.Q<Button>("AddButton");
        addButton.clicked += FolderControl;

        foreach (SnippetDatabase.FolderData folderData in SnippetDatabase.Instance.Folders)
        {
            AddFolderControl(folderData);
        }
    }
    private void OnGUI()
    {
        AdjustScrollViewSize();
    }

    private void AdjustScrollViewSize()
    {
        Rect scrollViewRect = new Rect(0, EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
        if (scrollView != null) scrollView.style.height = new StyleLength(scrollViewRect.height);
    }

    private void FolderControl()
    {
        AddFolderControl();
    }

    private void AddFolderControl(SnippetDatabase.FolderData folderData = null)
    {
        if (folderData == null)
        {
            int newFolderDataId = SnippetDatabase.Instance.GetNextFolderDataId();
            folderData = new SnippetDatabase.FolderData();
            folderData.id = newFolderDataId;
            folderData.name = "folder" + newFolderDataId;

            SnippetDatabase.Instance.Folders.Add(folderData);
            SnippetDatabase.Instance.Save();
        }

        FolderCustomControl newFolderControl = new FolderCustomControl();
        newFolderControl.Initialise(folderData.id);
        newFolderControl.m_labelField.text = folderData.name;

        newFolderControl.FolderButtonClicked += AddFolderSetting;
        newFolderControl.moveUpButton.clicked += ReloadHierarchy;
        newFolderControl.moveDownButton.clicked += ReloadHierarchy;
        newFolderControl.m_Button.clicked += RemoveLists;
        newFolderControl.m_folderSett.clicked += RemoveSnippets;

        scrollView.Add(newFolderControl);
        folderControlsList.Add(newFolderControl);
    }

    public void AddFolderSetting(int folderId)
    {
        SettingScroll.Clear();
        FolderSettingControl newFolderSettingControl = new FolderSettingControl(folderId);
        SettingScroll.Add(newFolderSettingControl);
        newFolderSettingControl.SnippetButtonClicked += AddSnippetControl;

    }

    private void AddSnippetControl(int snippetId, int folderId)
    {
        SnippetScroll.Clear();
        FolderSnippetControl newFolderSnippetControl = new FolderSnippetControl(snippetId, folderId);
         newFolderSnippetControl.SnippetMoved += ReloadSnippets; 
        SnippetScroll.Add(newFolderSnippetControl);

        newFolderSnippetControl.removeSnippet.clicked += () => ReloadSnippetList(folderId); 
    }

    private void ReloadHierarchy()
    {
        scrollView.Clear();
        scrollView.Add(addButton);

        foreach (SnippetDatabase.FolderData folderData in SnippetDatabase.Instance.Folders)
        {
            AddFolderControl(folderData);
        }
    }

    public void ReloadSnippets(int folderId, int snippetId)
    {
        scrollView.Clear();
        scrollView.Add(addButton);

        foreach (SnippetDatabase.FolderData folderData in SnippetDatabase.Instance.Folders)
        {
            AddFolderControl(folderData);
        }

        SettingScroll.Clear();
        FolderSettingControl folderSettingControl = new FolderSettingControl(folderId);
        folderSettingControl.SnippetButtonClicked += AddSnippetControl;
        SettingScroll.Add(folderSettingControl);

    }

    private void ReloadSnippetList(int folderId)
    {
        FolderSettingControl folderSettingControl = new FolderSettingControl(folderId);
        SettingScroll.Clear();
        SettingScroll.Add(folderSettingControl); 


    }

    private void RemoveLists()
    {
        SettingScroll.Clear();
        SnippetScroll.Clear(); 
    }

    private void RemoveSnippets()
    {
        SnippetScroll.Clear(); 
    }
}


