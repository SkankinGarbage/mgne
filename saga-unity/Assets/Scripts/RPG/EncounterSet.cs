using System.Collections.Generic;

public class EncounterSet : BaseEncounterSet {

    private EncounterSetData data;

    public EncounterSet(EncounterSetData data) {
        this.data = data;
    }

    protected override int AverageStepsToEncounter => data.steps;

    protected override List<EncounterSetMemberData> GetEncounters() {
        return new List<EncounterSetMemberData>(data.encounters);
    }
}
