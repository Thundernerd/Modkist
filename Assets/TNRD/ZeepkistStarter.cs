using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace TNRD
{
    public class ZeepkistStarter : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("steam://rungameid/1440670");
            p.Start();
            
            StartCoroutine(DisableButton());
        }

        private IEnumerator DisableButton()
        {
            button.interactable = false;
            yield return new WaitForSeconds(2.5f);
            button.interactable = true;
        }
    }
}
