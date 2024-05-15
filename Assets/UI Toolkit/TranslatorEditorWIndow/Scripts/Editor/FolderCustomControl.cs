using UnityEngine.UIElements;
using System;
using UnityEditor;
using UnityEngine;

public class FolderCustomControl : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FolderCustomControl, UxmlTraits> { }
    public Button m_folderSett;
    public Label m_labelField;
    public TextField m_SettingField;
    public Button m_Button;
    public Button moveUpButton;
    public Button moveDownButton;
    public ScrollView scrollView;
    int folderId = -1;
    public event Action<int> FolderButtonClicked;

    public FolderCustomControl()
    {

        string stylesheetPath = "Assets/UI Toolkit/TranslatorEditorWindow/Resources/Style/FolderCustomControls.uss";
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheetPath);
        Texture2D blueFolderTexture = Resources.Load<Texture2D>("Style/BlueFolder");

        var folderImage = new Image();
        folderImage.image = blueFolderTexture;
        folderImage.name = "ImageFolder";
        hierarchy.Add(folderImage);

        m_Button = new Button();
        m_Button.name = "Button";
        hierarchy.Add(m_Button);
        TextElement textElement = new TextElement();
        textElement.text = "Delete folder";
        m_Button.Add(textElement);
        m_Button.clicked += (ShowDeleteConfirmationPopup);

        m_labelField = new Label();
        m_labelField.name = "LabelField";
        hierarchy.Add(m_labelField);

        m_folderSett = new Button();
        m_folderSett.name = "FolderSettButton";
        m_folderSett.text = "FolderSettings";
        hierarchy.Add(m_folderSett);
        m_folderSett.clicked += () =>
        {
            FolderButtonClicked?.Invoke(folderId);
        }; 

        moveUpButton = new Button();
        moveUpButton.text = "Move Up";
        hierarchy.Add(moveUpButton);
        moveUpButton.clicked += (MoveFolderUp);

        moveDownButton = new Button();
        moveDownButton.text = "Move down";
        hierarchy.Add(moveDownButton);
        moveDownButton.clicked += (MoveFolderDown);
    }

    public void Initialise(int folderId)
    {
        this.folderId = folderId;
        UpdateMoveButtonsState();
    }

    void ShowDeleteConfirmationPopup()
    {
        bool dialogResult = EditorUtility.DisplayDialog("Delete Folder", "Are you sure you want to delete the folder?", "Yes", "Cancel");
        if (dialogResult)
        {
            DeleteFolder(folderId);
        }
    }

    private void DeleteFolder(int folderId)
    {
        SnippetDatabase.Instance.DeleteFolder(folderId);
        SnippetDatabase.Instance.Save();
        hierarchy.Clear();

    }

    private void MoveFolderUp()
    {
        SnippetDatabase.Instance.MoveFolderUp(folderId);
        SnippetDatabase.Instance.Save();
    }

    private void MoveFolderDown()
    {
        SnippetDatabase.Instance.MoveFolderDown(folderId);
        SnippetDatabase.Instance.Save();
    }
    private void UpdateMoveButtonsState()
    {
        int folderIndex = SnippetDatabase.Instance.GetFolderDataIndexByFolderId(folderId);
        // Debug.Log($"Folder ID {folderId} has index {folderIndex}");
        moveUpButton.SetEnabled(folderIndex > 0);
        moveDownButton.SetEnabled(folderIndex < SnippetDatabase.Instance.Folders.Count - 1);
    }
} 


