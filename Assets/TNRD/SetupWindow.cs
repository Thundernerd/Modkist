using System.IO;
using System.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TNRD
{
    internal class SetupWindow : MonoBehaviour
    {
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button folderButton;
        [SerializeField] private Button button;

        private void Awake()
        {
            folderButton.onClick.AddListener(FolderButtonClicked);
            button.onClick.AddListener(ButtonClicked);
            button.interactable = false;
        }

        private void FolderButtonClicked()
        {
            FileBrowser.ShowLoadDialog(OnSuccess,
                OnCancel,
                FileBrowser.PickMode.Folders,
                false,
                null,
                null,
                "Locate Zeepkist folder");
        }

        private void ButtonClicked()
        {
            SceneManager.LoadScene(sceneBuildIndex: 2);
        }

        private void OnSuccess(string[] paths)
        {
            string path = paths.First();
            input.text = path;
            Preferences.GamePath = path;
            button.interactable = true;
        }

        private void OnCancel()
        {
        }
    }
}
