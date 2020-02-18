using System.Threading.Tasks;

public class AIRandom : AIBase {

    public AIRandom(Unit unit) : base(unit) {

    }

    protected override async Task PopulateIntentAsync(Intent intent, Party allies, Party enemy) {
        intent.SetItem(intent.Actor.Equipment.SelectRandomBattleItem());
        if (intent.Item != null) {
            intent.Targets = await intent.Item.Effect.AcquireTargetsAsync(actor, intent.Battle, true);
        }
    }
}
