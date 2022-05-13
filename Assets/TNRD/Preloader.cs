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
            if (!string.IsNullOrEmpty(Preferences.GamePath) || TryAutoConfigure())
            {
                SceneManager.LoadScene(sceneBuildIndex: 2);
            }
            else
            {
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
                if (TryGetPathFromToken(token, out string steamGamesFolder))
                {
                    gameFolder = Path.Combine(steamGamesFolder, "steamapps", "common", "Zeepkist");
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetPathFromToken(VToken token, out string gameFolder)
        {
            gameFolder = null;
            if (token.Type != VTokenType.Property)
                return false;

            VProperty property = token.Value<VProperty>();
            if (property.Value.Type != VTokenType.Object)
                return false;

            VObject vObject = property.Value.Value<VObject>();
            if (!vObject.TryGetValue("apps", out VToken apps))
                return false;

            if (apps.Type != VTokenType.Object)
                return false;

            VObject appsObj = apps.Value<VObject>();
            if (appsObj.ContainsKey("1440670"))
            {
                if (vObject.TryGetValue("path", out VToken pathToken))
                {
                    gameFolder = pathToken.Value<VValue>().ToString();
                    return true;
                }
                else
                {
                    // This seems kinda problematic?
                }
            }

            return false;
        }
    }
}
