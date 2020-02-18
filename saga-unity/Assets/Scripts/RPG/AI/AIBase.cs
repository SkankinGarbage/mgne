using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public abstract class AIBase {

    protected Unit actor;

    public AIBase(Unit actor) {
        this.actor = actor;
    }

    public async Task<Intent> ConstructIntentAsync(Battle battle) {
        var isEnemy = battle.Enemy.Contains(actor);
        var enemies = isEnemy ? battle.Enemy : battle.Player;
        var allies = isEnemy ? battle.Player : battle.Enemy;

        Intent intent = new Intent(actor, battle);
        await PopulateIntentAsync(intent, allies, enemies);
        return intent;
    }

    protected abstract Task PopulateIntentAsync(Intent intent, Party allies, Party enemy);
}
