using UnityEngine;
using System;
using System.Text;

public static class UIUtils {

    public static void AttachAndCenter(GameObject parent, GameObject child) {
        RectTransform parentTransform = parent.GetComponent<RectTransform>();
        RectTransform childTransform = child.GetComponent<RectTransform>();
        childTransform.SetParent(parentTransform);
        childTransform.anchorMin = new Vector2(0.5f, 0.5f);
        childTransform.anchorMax = childTransform.anchorMin;
        childTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        childTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// <returns>The input string with ascii characters for glyphs replacing $A, $B etc</returns>
    public static string GlyphifyString(string input) {
        StringBuilder output = new StringBuilder();
        for (int i = 0; i < input.Length; i += 1) {
            if (input[i] == '$') {
                int ascii = input[i + 1];
                ascii += 232 - 'A'; // glyph $A starts at 232
                output.Append((char)ascii);
                i += 1;
            } else {
                output.Append(input[i]);
            }
        }
        return output.ToString();
    }

    public static DateTime TimestampToDateTime(long timestamp) {
        var offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return offset.DateTime;
    }

    public static long CurrentTimestamp() {
        var offset = DateTimeOffset.UtcNow;
        return offset.ToUnixTimeSeconds();
    }
}
