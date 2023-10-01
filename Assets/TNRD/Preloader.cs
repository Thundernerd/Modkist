using System;
using System.IO;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using JetBrains.Annotations;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNRD
{
    internal class Preloader : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                SceneManager.LoadScene(sceneBuildIndex: TryAutoConfigure() ? 2 : 1);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SceneManager.LoadScene(sceneBuildIndex: 1);
            }
            
        }

        private static bool TryAutoConfigure()
        {
            string steamPath = GetSteamPath();
            if (string.IsNullOrEmpty(steamPath))
                return false;

            if (!TryGetGameFolder(steamPath, out string gameFolder))
                return false;

            Preferences.GamePath = gameFolder;
            return true;
        }

        [CanBeNull]
        private static string GetSteamPath()
        {
            return Get32BitSteam() ?? Get64BitSteam();
        }

        [CanBeNull]
        private static string Get32BitSteam()
        {
            try
            {
                return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        private static string Get64BitSteam()
        {
            try
            {
                return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam",
                    "InstallPath",
                    null);
            }
            catch
            {
                return null;
            }
        }

        private static bool TryGetGameFolder(string steamFolder, out string gameFolder)
        {
            gameFolder = null;
            string libraryFoldersPath = Path.Combine(steamFolder, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(libraryFoldersPath))
                return false;

            string contents = File.ReadAllText(libraryFoldersPath);
            VProperty root = VdfConvert.Deserialize(contents);

            foreach (VToken token in root.Value)
            {
                if (!TryGetPathFromToken(token, out string steamGamesFolder)) 
                    continue;

                string manifestPath = Path.Combine(steamGamesFolder, "steamapps", "appmanifest_1440670.acf");
                string zeepkistPath = Path.Combine(steamGamesFolder, "steamapps", "common", "Zeepkist");
                if (File.Exists(manifestPath) && Directory.Exists(zeepkistPath))
                {
                    gameFolder = zeepkistPath;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetPathFromToken(VToken token, out string path)
        {
            path = null;
            if (token.Type != VTokenType.Property)
                return false;

            VProperty property = token.Value<VProperty>();
            if (property.Value.Type != VTokenType.Object)
                return false;

            VObject vObject = property.Value.Value<VObject>();
            if (!vObject.TryGetValue("path", out VToken pathToken))
                return false;

            if (pathToken.Type != VTokenType.Value)
                return false;

            VValue value = pathToken.Value<VValue>();
            path = value.Value.ToString();

            return true;
        }
    }
}
