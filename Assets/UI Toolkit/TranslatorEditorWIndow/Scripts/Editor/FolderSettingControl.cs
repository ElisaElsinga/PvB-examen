using System;
using UnityEditor;
using UnityEngine.UIElements;
using static SnippetDatabase;

public class FolderSettingControl : VisualElement
{
    private ScrollView scrollView;
    private int folderId = -1;
    private Button addSnippetButton;
    public Button snippetButton;
    public event Action<int, int> SnippetButtonClicked;


    public FolderSettingControl(int folderId)
    {
        this.folderId = folderId;
       // this.snippetId = snippetId; 

        string stylesheetPath = "Assets/UI Toolkit/TranslatorEditorWIndow/Resources/Style/FolderSettingControl.uss";
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheetPath);

        scrollView = new ScrollView();
        Add(scrollView);
        addSnippetButton = new Button();
        addSnippetButton.clicked += AddSnippet;
        addSnippetButton.text = "Add Snippet";
        scrollView.Add(addSnippetButton);
        DisplayFolderSettings(); 
        DisplaySnippets(); 
        
    }

    public void AddSnippet()
    {
        SnippetData newSnippet = new SnippetData();
        newSnippet.id = SnippetDatabase.Instance.GetNextSnippetDataId();
        newSnippet.name = "Snippet" + newSnippet.id;
        newSnippet.folderId = folderId;

      
        SnippetDatabase.Instance.Snippets.Add(newSnippet);
        SnippetDatabase.Instance.Save();

       UnityEngine.Debug.Log("Snippet Name: " + newSnippet.name);

        ReloadSnippets();
    }

    private void OnTextInput(ChangeEvent<string> evt)
    {
        SnippetDatabase.Instance.GetFolderDataByFolderId(folderId).name = evt.newValue;
        SnippetDatabase.Instance.Save();
    }

    private void DisplayFolderSettings()
    {
        scrollView.Clear();
        FolderData folderData = SnippetDatabase.Instance.GetFolderDataByFolderId(folderId);
        if (folderData != null)
        {
            var folderSettings = new VisualElement();
            folderSettings.Add(new Label("Folder Settings:"));

            TextField folderNameField = new TextField("Folder Name:");
            folderNameField.value = folderData.name;
            folderNameField.RegisterValueChangedCallback(OnTextInput);
            folderNameField.SetValueWithoutNotify(folderData.name);
            folderSettings.Add(folderNameField);
            folderNameField.name = "folderNameField"; 
            folderSettings.Add(addSnippetButton); 

            scrollView.Add(folderSettings);
        }
    }

    public void DisplaySnippets()
    {
        FolderData folderData = SnippetDatabase.Instance.GetFolderDataByFolderId(folderId);
        if (folderData != null)
        {
            foreach (SnippetData snippetData in SnippetDatabase.Instance.Snippets)
            {
                if (snippetData.folderId == folderData.id)
                {
                    snippetButton = new Button();
                    snippetButton.text = snippetData.name;
                    snippetButton.name = "snippets";
                    snippetButton.clicked += () =>
                    {
                        SnippetButtonClicked.Invoke(snippetData.id, folderId);
                    };
                    scrollView.Add(snippetButton);
                    UnityEngine.Debug.Log("Snippet Name: " + snippetData.name);
                }
            }
        }
    }

    public void ReloadSnippets()
    {
        scrollView.Clear();

        DisplayFolderSettings();
        DisplaySnippets(); 
    }

    

}





