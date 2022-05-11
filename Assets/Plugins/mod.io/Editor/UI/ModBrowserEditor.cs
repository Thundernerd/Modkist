﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace ModIO.UI.EditorCode
{
    [CustomEditor(typeof(ModBrowser))]
    public class ModBrowserEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool locateSettings = GUILayout.Button("Plugin Settings");
            if(locateSettings)
            {
                PluginSettings.FocusAsset();
            }
        }
    }
}
#endif
