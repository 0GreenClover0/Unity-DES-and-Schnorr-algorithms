using UnityEngine;
using AnotherFileBrowser.Windows;
using TMPro;

public class FileManager : MonoBehaviour
{
    public TMP_InputField encryptLoadPath;
    public TMP_InputField encryptSavePath;
    public TMP_InputField decryptLoadPath;
    public TMP_InputField decryptSavePath;

    public string encryptLoadPathValidated;
    public string encryptSavePathValidated;
    public string decryptLoadPathValidated;
    public string decryptSavePathValidated;

    private void OnEnable()
    {
        encryptLoadPath.onValueChanged.AddListener(OnEncryptLoadPathChange);
        encryptSavePath.onValueChanged.AddListener(OnEncryptSavePathChange);

        decryptLoadPath.onValueChanged.AddListener(OnDecryptLoadPathChange);
        decryptSavePath.onValueChanged.AddListener(OnDecryptSavePathChange);
    }

    private void OnDisable()
    {
        encryptLoadPath.onValueChanged.RemoveListener(OnEncryptLoadPathChange);
        encryptSavePath.onValueChanged.RemoveListener(OnEncryptSavePathChange);

        decryptLoadPath.onValueChanged.RemoveListener(OnDecryptLoadPathChange);
        decryptSavePath.onValueChanged.RemoveListener(OnDecryptSavePathChange);
    }

    private void OnEncryptLoadPathChange(string newValue)
    {
        encryptLoadPath.SetTextWithoutNotify(encryptLoadPathValidated);
    }

    private void OnEncryptSavePathChange(string newValue)
    {
        encryptSavePath.SetTextWithoutNotify(encryptSavePathValidated);
    }

    private void OnDecryptSavePathChange(string newValue)
    {
        decryptSavePath.SetTextWithoutNotify(decryptSavePathValidated);
    }

    private void OnDecryptLoadPathChange(string newValue)
    {
        decryptLoadPath.SetTextWithoutNotify(decryptLoadPathValidated);
    }

    public void OpenFileBrowserEncryptLoadPath()
    {
        var bp = new BrowserProperties();

        new FileBrowser().OpenFileBrowser(bp, path => {
            encryptLoadPath.SetTextWithoutNotify(path);
            encryptLoadPathValidated = path;
        });
    }

    public void OpenFileBrowserEncryptSavePath()
    {
        var bp = new BrowserProperties();

        new FileBrowser().SaveFileBrowser(bp, "savedFile", ".txt", path =>
        {
            encryptSavePath.SetTextWithoutNotify(path);
            encryptSavePathValidated = path;
        });
    }

    public void OpenFileBrowserDecryptSavePath()
    {
        var bp = new BrowserProperties();

        new FileBrowser().SaveFileBrowser(bp, "savedFile", ".txt", path =>
        {
            decryptSavePath.SetTextWithoutNotify(path);
            decryptSavePathValidated = path;
        });
    }

    public void OpenFileBrowserDecryptLoadPath()
    {
        var bp = new BrowserProperties();

        new FileBrowser().OpenFileBrowser(bp, path => {
            decryptLoadPath.SetTextWithoutNotify(path);
            decryptLoadPathValidated = path;
        });
    }
}
