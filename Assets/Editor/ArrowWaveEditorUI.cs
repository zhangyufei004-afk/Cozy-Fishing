#if UNITY_EDITOR

using FishingGame.Reeling;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FishingGame.CustomEditors
{
    public class ArrowWaveEditorUI : EditorWindow
    {

        private ArrowWaveSO _waveData;

        [MenuItem("Cozy Fishing/Minigames/Arrow Minigame Editor")]
        public static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(ArrowWaveEditorUI));
        }

        private void OnGUI()
        {
            if (_waveData is null)
            {
                GUILayout.Label("Please select a arrow minigame data", EditorStyles.largeLabel);
                return;
            }

            GUILayout.Label("Good to go!");

            foreach (ArrowWaveEntry arrowWaveEntry in _waveData.ArrowEntrys)
            {
               // GUILayout.
            }

        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is ArrowWaveSO)
            {
                _waveData = Selection.activeObject as ArrowWaveSO;
            }
            else
            {
                _waveData = null;
            }
        }


    }


}


#endif