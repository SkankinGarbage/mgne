using System;

public enum Gender {
    [Gender("M")] MALE,
    [Gender("F")] FEMALE,
    [Gender("")] NONE,
}

public class GenderAttribute : Attribute {

    public string label;

    internal GenderAttribute(string label) {
        this.label = label;
    }
}

public static class GenderExtensions {

    public static string Label(this Gender gender) { return gender.GetAttribute<GenderAttribute>().label; }

}
