using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseEncounterSet {

    private const string StepsSinceEncounterVariable = "steps_since_encounter";

    protected abstract int AverageStepsToEncounter { get; }

    public void CheckForEncounterOnStep() {
        var steps = Global.Instance().Data.GetVariable(StepsSinceEncounterVariable);
        Global.Instance().Data.IncrementVariable(StepsSinceEncounterVariable);

        // valid? maybe we're not on the right terrain
        if (steps <= 0) {
            return;
        }

        // have to go at least half the average steps
        if (steps < AverageStepsToEncounter / 2) {
            return;
        }

        // after that you're equally likely to see an encounter each step
        if (Random.Range(0, AverageStepsToEncounter / 2) > 0) {
            return;
        }

        // okay, you should see something
        var encounters = GetEncounters();
        var totalWeight = encounters.Aggregate(0, (acc, data) => acc + data.weight);
        var roll = Random.Range(0, totalWeight);
        EncounterSetMemberData encounter = null;
        foreach (var possible in encounters) {
            if (roll < possible.weight) {
                encounter = possible;
                break;
            } else {
                roll -= possible.weight;
            }
        }

        var party = new Party(encounter.encounter);
        Global.Instance().Data.SetVariable(StepsSinceEncounterVariable, 0);
        Global.Instance().Maps.Avatar.PauseInput();
        Global.Instance().StartCoroutine(CoUtils.RunWithCallback(BattleView.SpawnBattleRoutine(party), () => {
            Global.Instance().Maps.Avatar.UnpauseInput();
        }));
    }

    protected abstract List<EncounterSetMemberData> GetEncounters();
}
