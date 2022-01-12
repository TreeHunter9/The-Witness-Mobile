#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyEditor
{
    [CustomEditor(typeof(MainPuzzleCreator))]
    public class ButtonForCreateLevel : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            MainPuzzleCreator script = (MainPuzzleCreator) target;

            if (GUILayout.Button("Create Level"))
            {
                script.CreateWholeLevel();
            }
        }
    }
}
#endif
