using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using System.IO;
using SK.Libretro.Unity;
using static SnippetDatabase;
using System;
using UnityEditor;

public class FolderSnippetControl : VisualElement
{
    private Rect selectionRect;
    private Vector2 selectionStart;
    private bool isSelecting;

    private VisualElement selectionOverlay; // For displaying the selection box

    public Button moveUpButton;
    public Button moveDownButton;
    int snippetId = -1;
    public Button UploadImage;
    private Image screenshotImage;
    private Button removeSnippet;
    public TextField snippetName;
    public Button loadSnippetState;
    private Label labelImageSnippet;
    private Label localizedTextName; 
    private TextField localizedText;
    private Label japaneseStringLabel; 
    public event Action<int, int> SnippetMoved;


    int folderId = -1;

    [SerializeField] private LibretroInstanceVariable _libretro;

    public FolderSnippetControl(int snippetId, int folderId)
    {

        this.folderId = folderId;

        string stylesheetPath = "Assets/UI Toolkit/TranslatorEditorWIndow/Resources/Style/FolderSnippetControl.uss";
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheetPath);

        this.snippetId = snippetId;

        snippetName = new TextField("Snippet name: ");
        snippetName.name = "SnippetName";
        hierarchy.Add(snippetName);
        snippetName.RegisterValueChangedCallback(OnTextInput);

        removeSnippet = new Button();
        removeSnippet.text = "Delete snippet";
        removeSnippet.name = "DeleteSnippet";
        removeSnippet.clicked += ShowDeleteConfirmationPopup;
        hierarchy.Add(removeSnippet);

        moveUpButton = new Button();
        moveUpButton.text = "Move Up";
        moveUpButton.name = "MoveUpButton";
        moveUpButton.clicked += MoveSnippetUp;
        hierarchy.Add(moveUpButton);

        moveDownButton = new Button();
        moveDownButton.text = "Move Down";
        moveDownButton.name = "MoveDownButton";
        moveDownButton.clicked += MoveSnippetDown;
        hierarchy.Add(moveDownButton);

        labelImageSnippet = new Label();
        labelImageSnippet.text = "Snippet image:";
        labelImageSnippet.name = "labelSnippetImage";
        hierarchy.Add(labelImageSnippet);

        UploadImage = new Button();
        UploadImage.text = "Create an snippet image";
        UploadImage.name = "UploadButton";
        UploadImage.clicked += () =>
        {
            CreateSnippet(snippetId);

            EditorApplication.delayCall += () =>
            {
               
                OpenFiles(snippetId);
            };
        };
        hierarchy.Add(UploadImage);

        screenshotImage = new Image();
        screenshotImage.name = "screenshot";
        screenshotImage.RegisterCallback<MouseDownEvent>(OnScreenshotMouseDown);
        screenshotImage.RegisterCallback<MouseMoveEvent>(OnScreenshotMouseMove);
        screenshotImage.RegisterCallback<MouseUpEvent>(OnScreenshotMouseUp);
        screenshotImage.RegisterCallback<MouseLeaveEvent>(OnScreenshotMouseLeave);
        hierarchy.Add(screenshotImage);

        loadSnippetState = new Button();
        loadSnippetState.text = "Load snippet state";
        loadSnippetState.name = "loadSnippetState";
        loadSnippetState.clicked += () => LoadSaveState(snippetId);
        hierarchy.Add(loadSnippetState);


        selectedAreaImage = new Image();
        selectedAreaImage.name = "SelectedAreaImage";
        hierarchy.Add(selectedAreaImage);

        japaneseStringLabel = new Label();
        japaneseStringLabel.text = "Japanese string:";
   


        DisplayName();
        InitializeSelectionOverlay();
    }

    private void OnTextInput(ChangeEvent<string> evt)
    {
        SnippetDatabase.Instance.GetSnippetDataBySnippetId(snippetId).name = evt.newValue;
        SnippetDatabase.Instance.Save();
    }

    private void DisplayName()
    {
        SnippetData snippetData = SnippetDatabase.Instance.GetSnippetDataBySnippetId(snippetId);
        if (snippetData != null)
        {
            snippetName.value = snippetData.name;
            snippetName.RegisterValueChangedCallback(OnTextInput);
            snippetName.SetValueWithoutNotify(snippetData.name);
            // Get the raw texture data from selectedTexture
            Texture2D selectedTexture = new Texture2D(snippetData.spriteWidth, snippetData.spriteHeight);
            selectedTexture.LoadRawTextureData(snippetData.spriteData);
            selectedTexture.Apply();

            // Update selectedAreaImage's sprite with the loaded texture from spriteData
            selectedAreaImage = new Image();
            selectedAreaImage.name = "SelectedAreaImage";
            selectedAreaImage.sprite = Sprite.Create(selectedTexture, new Rect(0, 0, selectedTexture.width, selectedTexture.height), Vector2.one * 0.5f);

            selectedAreaImage.style.width = selectedTexture.width * 2;
            selectedAreaImage.style.height = selectedTexture.height * 2;

            hierarchy.Add(selectedAreaImage);


            Debug.Log("SelectedAreaImage updated with sprite: " + selectedAreaImage.sprite);

            japaneseStringLabel = new Label();
            japaneseStringLabel.text = "Japanese string:";
            japaneseStringLabel.name = "JapaneseStringLabel";
            hierarchy.Add(japaneseStringLabel); 

            localizedTextName = new Label();
            localizedTextName.text = "TextField: localized text";
            localizedTextName.name = "LocalizedName";
            hierarchy.Add(localizedTextName); 

            localizedText = new TextField();
            localizedText.name = "LocalizedTextField";
            hierarchy.Add(localizedText);

            SnippetDatabase.Instance.Save();
            OpenFiles(snippetId);
        }
        else
        {
            Debug.Log("SnippetData is null.");
        }
    }



    private void CreateSnippet(int snippetId)
    {

        if (UnityEngine.Application.isPlaying)
        {
            Mainloop.Instance._libretro.SaveState(snippetId);
            Debug.Log(snippetId);
        }
    }

    private void OpenFiles(int snippetId)
    {

        string snippetImageName = "save_" + snippetId + ".png";
        string filePath = Path.Combine(Application.streamingAssetsPath, "libretro~", "states", "np2kai", "Nostalgia 1907 (System disk)", snippetImageName);

        if (System.IO.File.Exists(filePath))
        {
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                Sprite snippetSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                screenshotImage.style.width = texture.width * 2;
                screenshotImage.style.height = texture.height * 2;
                screenshotImage.sprite = snippetSprite;
            }
        }
    }

    private void MoveSnippetUp()
    {
        SnippetDatabase.Instance.MoveSnippetUp(snippetId);
        SnippetDatabase.Instance.Save();
        SnippetMoved?.Invoke(folderId, snippetId);
    }

    private void MoveSnippetDown()
    {
        SnippetDatabase.Instance.MoveSnippetDown(snippetId);
        SnippetDatabase.Instance.Save();
        SnippetMoved?.Invoke(folderId, snippetId);
    }


    private void ShowDeleteConfirmationPopup()
    {
        bool dialogResult = EditorUtility.DisplayDialog("Delete Snippet", "Are you sure you want to delete the snippet?", "Yes", "Cancel");
        if (dialogResult)
        {
            DeleteSnippet();
        }
    }

    private void DeleteSnippet()
    {
        SnippetDatabase.Instance.DeleteSnippet(snippetId);
    }

    private void LoadSaveState(int snippetId)
    {
        if (UnityEngine.Application.isPlaying)
        {
            Mainloop.Instance._libretro.LoadState(snippetId);
        }
    }

    private void InitializeSelectionOverlay()
    {
        selectionOverlay = new VisualElement();
        selectionOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0.5f)); 
        screenshotImage.Add(selectionOverlay);
        selectionOverlay.style.position = Position.Relative; // to Adjust the position mode
        selectionOverlay.style.left = 0f;
        selectionOverlay.style.top = 0f;
        selectionOverlay.style.width = 0f;
        selectionOverlay.style.height = 0f;
        selectionOverlay.style.display = DisplayStyle.None; // to hide it at first
    }

    // Makes a well formed rect out of a starting point and dimensions which could be negative
    static Rect MakeSelectionArea(Vector2 selectionStart, Vector2 selectionDimensions)
    {
        // Calculate min and max corners of the selection rect
        Vector2 min = new Vector2(Mathf.Min(selectionStart.x, selectionStart.x + selectionDimensions.x), Mathf.Min(selectionStart.y, selectionStart.y + selectionDimensions.y));
        Vector2 max = new Vector2(Mathf.Max(selectionStart.x, selectionStart.x + selectionDimensions.x), Mathf.Max(selectionStart.y, selectionStart.y + selectionDimensions.y));

        return new Rect(min, max - min);
    }

    private void OnScreenshotMouseDown(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            isSelecting = true;
            selectionStart = evt.localMousePosition;
            selectionRect = new Rect(selectionStart, Vector2.zero);
            selectionOverlay.style.display = DisplayStyle.Flex; // Show the selection overlay

            // Resize so the last selected region won't show
            selectionOverlay.style.left = evt.localMousePosition.x;
            selectionOverlay.style.top = evt.localMousePosition.y;
            selectionOverlay.style.width = 0;
            selectionOverlay.style.height = 0;

            evt.StopPropagation();
        }
    }

    private void OnScreenshotMouseMove(MouseMoveEvent evt)
    {
        if (isSelecting)
        {
            // Find area that was selected in screenshot image ui coordinates
            Rect selectedRect = MakeSelectionArea(selectionStart, evt.localMousePosition - selectionStart);

            // Apply selection rect size
            selectionOverlay.style.left = selectedRect.x;
            selectionOverlay.style.top = selectedRect.y;
            selectionOverlay.style.width = selectedRect.width;
            selectionOverlay.style.height = selectedRect.height;

            evt.StopPropagation();
        }
    }

    private Image selectedAreaImage;
    private void OnScreenshotMouseUp(MouseUpEvent evt)
    {
        if (evt.button == 0 && isSelecting)
        {
            isSelecting = false;
            selectionOverlay.style.display = DisplayStyle.None;
            Rect selectedRect = MakeSelectionArea(selectionStart, evt.localMousePosition - selectionStart);
            evt.StopPropagation();

            // Calculate selected area relative to UI screenshot image size
            Vector2 screenshotUIImageSize = new Vector2(screenshotImage.style.width.value.value, screenshotImage.style.height.value.value);
            Debug.Log($"Screenshot UI Image Size: {screenshotUIImageSize}");

            Rect selectedAreaLocal = new Rect(selectedRect.x / screenshotUIImageSize.x, selectedRect.y / screenshotUIImageSize.y,
                selectedRect.width / screenshotUIImageSize.x, selectedRect.height / screenshotUIImageSize.y);
            Debug.Log($"Selected Area Local: {selectedAreaLocal}");

            // Calculate selected image pixels in actual pixel coordinates within the screenshot image
            Vector2Int screenshotSize = new Vector2Int(screenshotImage.sprite.texture.width, screenshotImage.sprite.texture.height);
            Debug.Log($"Screenshot Size: {screenshotSize}");

            RectInt selectedAreaPixels = new RectInt(
                Mathf.FloorToInt(selectedAreaLocal.x * screenshotSize.x),
                Mathf.FloorToInt(selectedAreaLocal.y * screenshotSize.y),
                Mathf.FloorToInt(selectedAreaLocal.width * screenshotSize.x),
                Mathf.FloorToInt(selectedAreaLocal.height * screenshotSize.y)
            );

            //flip y coordinate in selected pixels for cutting out the texture since y=0 is at the bottom in texture coords
            RectInt selectedAreaPixelsTexture = selectedAreaPixels;
            selectedAreaPixelsTexture.y = screenshotSize.y - selectedAreaPixelsTexture.height - selectedAreaPixelsTexture.y;

            // Cut out the selected area from the screenshot
            Texture2D fullTexture = screenshotImage.sprite.texture;
            Texture2D selectedTexture = new Texture2D(selectedAreaPixelsTexture.width, selectedAreaPixelsTexture.height);
            Color[] pixels = fullTexture.GetPixels(selectedAreaPixelsTexture.x, selectedAreaPixelsTexture.y, selectedAreaPixelsTexture.width, selectedAreaPixelsTexture.height);
            selectedTexture.SetPixels(pixels);
            selectedTexture.Apply();

            // Store the width and height of the selected texture
            int selectedTextureWidth = selectedTexture.width;
            int selectedTextureHeight = selectedTexture.height;
            Debug.Log($"Selected Texture Width: {selectedTextureWidth}, Height: {selectedTextureHeight}");

            // Store the cut-out image pixels inside snippetData.spriteData
            byte[] spriteData = selectedTexture.GetRawTextureData();
            SnippetData snippetData = SnippetDatabase.Instance.GetSnippetDataBySnippetId(snippetId);
            if (snippetData != null)
            {
                snippetData.spriteData = spriteData;
                snippetData.spriteWidth = selectedTextureWidth;
                snippetData.spriteHeight = selectedTextureHeight;
                SnippetDatabase.Instance.Save();
                Debug.Log("Cut-out image pixels saved to database.");
            }

            if (selectedAreaImage != null)
            {
                hierarchy.Remove(selectedAreaImage);
                hierarchy.Remove(localizedText);
                hierarchy.Remove(localizedTextName);
                hierarchy.Remove(japaneseStringLabel); 
                selectedAreaImage = null;
            }

            // Create a new Image element for the selected area
            selectedAreaImage = new Image();
            selectedAreaImage.sprite = Sprite.Create(selectedTexture, new Rect(0, 0, selectedTextureWidth, selectedTextureHeight), Vector2.one * 0.5f);
            selectedAreaImage.name = "SelectedAreaImage";
            hierarchy.Add(selectedAreaImage);

            // Set the size of the selected area image to match the selected texture size
            selectedAreaImage.style.width = selectedTextureWidth;
            selectedAreaImage.style.height = selectedTextureHeight;

            japaneseStringLabel = new Label();
            japaneseStringLabel.text = "Japanese string:";
            japaneseStringLabel.name = "JapaneseStringLabel";
            hierarchy.Add(japaneseStringLabel);

            localizedTextName = new Label();
            localizedTextName.text = "TextField: localized text";
            localizedTextName.name = "LocalizedName";
            hierarchy.Add(localizedTextName);

            localizedText = new TextField();
            localizedText.name = "LocalizedTextField";
            hierarchy.Add(localizedText);
        }
    }


    private void OnScreenshotMouseLeave(MouseLeaveEvent evt)
    {
        // Cancel selection if mouse leaves the screenshot image
        isSelecting = false;
        selectionOverlay.style.display = DisplayStyle.None;
    }
} 
