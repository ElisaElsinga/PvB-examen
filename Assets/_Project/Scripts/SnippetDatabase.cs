using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class SnippetDatabase : ScriptableObject
{
    static string assetName = nameof(SnippetDatabase);
    static SnippetDatabase instance;

    [SerializeField]
    int folderIdCount = 0;

    [SerializeField]
    int snippetIdCount = 0;


    [Serializable]
    public class FolderData
    {
        [SerializeField]
        public int id = -1;

        [SerializeField]
        public string name = string.Empty;

    }

    [Serializable]
    public class SnippetData
    {
        [SerializeField]
        public int id = -1;

        [SerializeField]
        public string name = string.Empty;

        [SerializeField]
        public int folderId = -1;

        [SerializeField]
        public byte[] spriteData;

        [SerializeField]
        public int spriteWidth; 

        [SerializeField]
        public int spriteHeight;

    }
    [SerializeField]
    List<SnippetData> snippets = new List<SnippetData>();

    [SerializeField]
    List<FolderData> folders = new List<FolderData>();


    public static SnippetDatabase Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                instance = Resources.Load<SnippetDatabase>(assetName);
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    instance = CreateInstance<SnippetDatabase>();
#if UNITY_EDITOR
                    AssetDatabase.CreateAsset(instance, $"Assets/Resources/{assetName}.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
#endif
                    return instance;
                }
            }
        }
    }

    public List<FolderData> Folders { get { return folders; } }
    public List<SnippetData> Snippets { get { return snippets; } }

    public FolderData GetFolderDataByFolderId(int folderId)
    {
        foreach (var folder in folders)
        {
            if (folder.id == folderId)
                return folder;
        }

        return null;
    }

    public int GetFolderDataIndexByFolderId(int folderId)
    {
        for (int iFolder = 0; iFolder < folders.Count; iFolder++)
        {
            FolderData folderData = folders[iFolder];
            if (folderData.id == folderId)
                return iFolder;
        }

        return -1;
    }

    public int GetNextFolderDataId()
    {
        folderIdCount++;
        Save();
        return folderIdCount;
    }

    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public void DeleteFolder(int folderId)
    {
        FolderData folderToDelete = folders.Find(folder => folder.id == folderId);
        folders.Remove(folderToDelete);
        Save();
    }

    // This is used to move the folder with a specific folderId in the database up one spot
    public void MoveFolderUp(int folderId)
    {
        // Initialize folderIndex to the position in the list of folders of the folder with folderId
        int folderIndex = GetFolderDataIndexByFolderId(folderId);

        if (folderIndex > 0 && folderIndex < folders.Count)
        {
            FolderData position = folders[folderIndex];
            folders[folderIndex] = folders[folderIndex - 1];
            folders[folderIndex - 1] = position;
        }
    }

    // This is used to move the folder with a specific folderId in the database down one spot
    public void MoveFolderDown(int folderId)
    {
        int folderIndex = GetFolderDataIndexByFolderId(folderId);

        if (folderIndex >= 0 && folderIndex < folders.Count - 1)
        {
            FolderData position = folders[folderIndex];
            folders[folderIndex] = folders[folderIndex + 1];
            folders[folderIndex + 1] = position;
            Debug.Log(folderIndex); 
        } 
    }

    public SnippetData GetSnippetDataBySnippetId(int snippetId)
    {
        foreach (var snippet in snippets)
        {
            if (snippet.id == snippetId)
                return snippet;
        }

        return null;
    }

    public int GetSnippetDataIndexBySnippetId(int snippetId)
    {
        for (int iSnippet = 0; iSnippet < snippets.Count; iSnippet++)
        {
            SnippetData snippetData = snippets[iSnippet];
            if (snippetData.id == snippetId)
                return iSnippet;
        }

        return -1;
    }

    public void MoveSnippetUp(int snippetId)
    {
        int snippetIndex = GetSnippetDataIndexBySnippetId(snippetId);

        if (snippetIndex > 0 && snippetIndex < snippets.Count)
        {
            SnippetData position = snippets[snippetIndex];
            snippets[snippetIndex] = snippets[snippetIndex - 1];
            snippets[snippetIndex - 1] = position;
        }
    }

    public void MoveSnippetDown(int snippetId)
    {
        int snippetIndex = GetSnippetDataIndexBySnippetId(snippetId);

        if (snippetIndex >= 0 && snippetIndex < snippets.Count - 1)
        {
            SnippetData position = snippets[snippetIndex];
            snippets[snippetIndex] = snippets[snippetIndex + 1];
            snippets[snippetIndex + 1] = position;
            Debug.Log(snippetIndex);
        }
       
    }
    public int GetNextSnippetDataId()
    {
        snippetIdCount++;
        Save();
        return snippetIdCount;

    }

    public void DeleteSnippet(int snippetId)
    {
        SnippetData snippetToDelete = snippets.Find(snippet => snippet.id == snippetId);
        snippets.Remove(snippetToDelete);
        Save();
    }

}


