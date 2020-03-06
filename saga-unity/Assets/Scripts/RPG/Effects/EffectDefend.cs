using System;
using System.Threading.Tasks;

public class EffectDefend : EffectAllyTarget {

    protected new EffectDefendData data;
    protected bool silent;
    protected string defendName;

    public int ShieldDodgeBonus => data.shielding;
    public override bool IsMapUsable() => false;

    public EffectDefend(EffectDefendData data, CombatItem item) : base(data, item) {
        this.data = data;

        if (data.defendName?.Length > 0) {
            defendName = UIUtils.GlyphifyString(data.defendName);
        } else if (item != null) {
            defendName = item.Name;
        } else {
            silent = true;
        }
    }

    public override void OnRoundStart(Intent intent) {
        base.OnRoundStart(intent);

        foreach (var target in intent.Targets) {
            intent.Battle.AddBoost(target, new StatSet(data.stats), oneRoundOnly:true);
            intent.Battle.AddDefense(target, this);
        }
    }

    public override async Task ResolveAsync(Intent intent) {
        string targetName = null;
        if (!silent) {
            switch (data.projector) {
                case AllyProjectorType.ALLY_PARTY:
                    targetName = "all ";
                    break;
                case AllyProjectorType.PLAYER_PARTY_ENEMY_GROUP:
                    targetName = "group ";
                    break;
                case AllyProjectorType.SINGLE_ALLY:
                    targetName = intent.Targets[0].Name + " ";
                    break;
                case AllyProjectorType.USER:
                    targetName = "";
                    break;
            }
            await PlayBattleAnimAsync(intent);
            await intent.Battle.WriteLineAsync(intent.Actor.Name + " defends " + targetName + " by " + defendName + ".");
        }
    }

    public override async Task OnBlock(Battle battle, Unit user) {
        var tab = BattleBox.Tab;
        var victimname = user.Name;
        await battle.WriteLineAsync(tab + victimname + " deflects by " + defendName + ".");
    }

    public DefendResult DefendAgainst(Intent attackIntent, Unit target, DamageType damType) {
        // TODO:
        return new DefendResult() {
            callback = null,
            countered = false,
        };
    }

    public class DefendResult {
        public Action<Intent, Unit> callback;
        public bool countered;
    }
}