using System;

public enum Race {
    [Race("Human")]     HUMAN,
    [Race("Mutant")]    MUTANT,
    [Race("Robot")]     ROBOT,
    [Race("Monster")]   MONSTER,
}

public class RaceAttribute : Attribute {

    public string name;

    internal RaceAttribute(string name) {
        this.name = name;
    }
}

public static class RaceExtensions { 

    public static string Name(this Race race) { return race.GetAttribute<RaceAttribute>().name; }

}
