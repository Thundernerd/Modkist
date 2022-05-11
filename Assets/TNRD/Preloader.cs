using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNRD
{
    internal class Preloader : MonoBehaviour
    {
        private void Start()
        {
            if (string.IsNullOrEmpty(Preferences.GamePath))
            {
                SceneManager.LoadScene(sceneBuildIndex: 1);
            }
            else
            {
                SceneManager.LoadScene(sceneBuildIndex: 3);
            }
        }
    }
}
