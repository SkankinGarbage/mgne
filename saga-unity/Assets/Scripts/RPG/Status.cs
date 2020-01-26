using UnityEngine;
using System.Collections;

public class Status {

    public StatusData Data { get; private set; }

    public bool IsDeadly => Data.lethality == LethalityType.DEADLY || Data.lethality == LethalityType.DEATH;
    public override string ToString() => Data.fullName;

    public Status(StatusData data) {
        Data = data;
    }
}
