using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TNRD
{
    internal class BepInExInstaller : MonoBehaviour
    {
        private const string URL =
            "https://github.com/BepInEx/BepInEx/releases/download/v5.4.19/BepInEx_x64_5.4.19.0.zip";

        private IEnumerator Start()
        {
            if (HasBepInEx())
            {
                LoadScene();
                yield break;
            }

            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            UnityWebRequest webRequest = UnityWebRequest.Get(URL);
            yield return webRequest.SendWebRequest();


            try
            {
                string bepInExZipPath = Path.Combine(tempDirectory, "BepInEx.zip");
                File.WriteAllBytes(bepInExZipPath, webRequest.downloadHandler.data);

                using (ZipFile zipFile = ZipFile.Read(bepInExZipPath))
                {
                    zipFile.ExtractAll(Preferences.GamePath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }

            LoadScene();
        }

        private void LoadScene()
        {
            SceneManager.LoadScene(sceneBuildIndex: 3);
        }

        private bool HasBepInEx()
        {
            string dllPath = Path.Combine(Preferences.GamePath, "BepInEx", "core", "BepInEx.dll");

            if (File.Exists(dllPath))
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(dllPath);
                return fileVersionInfo.FileVersion == "5.4.19.0";
            }

            return false;
        }
    }
}
