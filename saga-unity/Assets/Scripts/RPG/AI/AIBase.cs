using UnityEngine;
using System.Collections;

public abstract class AIBase {

    protected Unit actor;

    public AIBase(Unit actor) {
        this.actor = actor;
    }

    public Intent ConstructIntent(Battle battle) {
        var isEnemy = battle.Enemy.Contains(actor);
        var enemies = isEnemy ? battle.Enemy : battle.Player;
        var allies = isEnemy ? battle.Player : battle.Enemy;

        Intent intent = new Intent(actor, battle);
        PopulateIntent(intent, allies, enemies);
        return intent;
    }

    protected abstract void PopulateIntent(Intent intent, Party allies, Party enemy);
}
