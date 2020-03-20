using System.Threading.Tasks;

public class EffectStatus : EffectEnemyTarget {

    protected new EffectStatusData data;

    private Status status;

    public EffectStatus(EffectStatusData data, CombatItem item) : base(data, item) {
        this.data = data;
        status = new Status(data.status);
    }

    protected override async Task ApplyEffect(Intent intent, Unit target, int power) {
        await target.TryApplyStatusAsync(status, intent.Battle);
    }

    protected override Task<int> CalculateAttackPowerAsync(Intent intent) {
        var power = data.hit;
        if (data.accStat != StatTag.NONE) {
            power += intent.Actor[data.accStat];
        }
        return Task.FromResult(power);
    }

    protected override async Task<bool> CheckHitAsync(Intent intent, Unit target, int power, float roll) {
        var tab = BattleBox.Tab;
        var temp = power;

        if (target.IsResistantTo(status)) {
            temp -= 255;
        }
        if (data.evadeStat != StatTag.NONE) {
            temp -= target[data.evadeStat];
        }
        float chance = temp / 100f;
        var hit = roll < chance;
        if (hit) {
            return true;
        } else {
            await intent.Battle.WriteLineAsync(tab + "Nothing happens.");
            return false;
        }
    }
}