using UnityEditor;
using UnityEngine;

public static class DomainReloadToggle
{
    [MenuItem("Tools/Misc/Toggle Domain Reload")]
    public static void Toggle()
    {
        var current = EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
        var newValue = !current;

        if (newValue)
            EditorSettings.enterPlayModeOptions |= EnterPlayModeOptions.DisableDomainReload;
        else
            EditorSettings.enterPlayModeOptions &= ~EnterPlayModeOptions.DisableDomainReload;

        EditorSettings.enterPlayModeOptionsEnabled = EditorSettings.enterPlayModeOptions != EnterPlayModeOptions.None;

        Debug.Log("Domain Reload " + (newValue ? "Disabled" : "Enabled"));
    }
}