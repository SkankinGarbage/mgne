using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class UIUtils {

    public static void AttachAndCenter(GameObject parent, GameObject child) {
        RectTransform parentTransform = parent.GetComponent<RectTransform>();
        RectTransform childTransform = child.GetComponent<RectTransform>();
        childTransform.SetParent(parentTransform);
        childTransform.anchorMin = new Vector2(0.5f, 0.5f);
        childTransform.anchorMax = childTransform.anchorMin;
        childTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        childTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static DateTime TimestampToDateTime(double timestamp) {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
        return dtDateTime;
    }

    public static double CurrentTimestamp() {
        return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
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
}
