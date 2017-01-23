using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(License), true)]
public class LicenseEditor : Editor
{
    private SerializedProperty mLicenseKey;

    //Method
    void OnEnable()
    {
        this.mLicenseKey = base.serializedObject.FindProperty("mLicenseKey");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(this.mLicenseKey, new GUIContent("App License Key"), new GUILayoutOption[] { GUILayout.MinHeight(40f), GUILayout.MaxHeight(100f) });
        this.mLicenseKey.stringValue = this.mLicenseKey.stringValue.Replace(" ", "").Replace("\n", "").Replace("\r", "");
        EditorStyles.textField.wordWrap = true; // 自动换行

        base.serializedObject.ApplyModifiedProperties();
    }
}
