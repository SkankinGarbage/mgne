using UnityEngine;
using System.Collections;

public class Status {

    public StatusData Data { get; private set; }

    public bool IsDeadly => Data.lethality == LethalityType.DEADLY || Data.lethality == LethalityType.DEATH;
    public override string ToString() => Data.fullName;

    public Status(StatusData data) {
        Data = data;
    }

    /// <summary>Remove the status off the victim, out of battle</summary>
    public void Heal(Unit victim) {
        if (Data.lethality == LethalityType.DEATH) {
            victim.Heal(1);
        }
        victim.Status = null;
    }
}
