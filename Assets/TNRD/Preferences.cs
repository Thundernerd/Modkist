using UnityEngine;

namespace TNRD
{
    public class Preferences
    {
        public static string GamePath
        {
            get => PlayerPrefs.GetString("TNRD.Modkist.GamePath");
            set
            {
                PlayerPrefs.SetString("TNRD.Modkist.GamePath", value);
                PlayerPrefs.Save();
            }
        }
    }
}
