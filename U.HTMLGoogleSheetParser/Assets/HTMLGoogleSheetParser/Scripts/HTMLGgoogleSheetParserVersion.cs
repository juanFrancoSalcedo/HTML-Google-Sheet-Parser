using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[InitializeOnLoad]
public static class HTMLGgoogleSheetParserVersion
{
    static string versionKey = "GoogleParserVersion_Shown";
    static string version = "0.1.5";
    static HTMLGgoogleSheetParserVersion()
    {
        if (!SessionState.GetBool(versionKey, false))
        {
            Debug.Log($"HTML Google Sheet Parser Version {version}");
            SessionState.SetBool(versionKey, true);
        }
    }
}
#endif
