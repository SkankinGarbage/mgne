using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Warhead superclass
/// </summary>
public abstract class AbilEffect {

    protected AbilEffectData data;
    protected CombatItem item;

    public CombatItem Item => item;
    
    protected AbilEffect(AbilEffectData data, CombatItem item) {
        this.data = data;
        this.item = item;
    }

    public abstract bool IsBattleUsable();
    public abstract bool IsMapUsable();
    public virtual bool CanTargetDead() => false;

    public virtual Task UseOnMapAsync(IItemUseableMenu menu, Unit user) {
        Debug.LogError("Unimplemented map use for item " + this);
        return Task.FromResult(0);
    }

    public void PlayMainSound() {
        // TODO: extract audio from battle anim
    }

    public virtual void OnRoundStart(Intent intent) {
        // default is nothing
    } 

    public virtual Task OnBlock(Battle battle, Unit user) {
        Debug.LogError("Unimplemented block for item " + this);
        return Task.FromResult(0);
    }

    public virtual Task<List<Unit>> AcquireTargetsAsync(Unit actor, Battle battle, bool useAI) {
        Debug.LogError("Unimplemented combat use for item " + this);
        return null;
    }

    public virtual List<Unit> AcquireRandomTargets(Unit actor, Battle battle) {
        return new List<Unit>();
    }

    public virtual async Task ResolveAsync(Intent intent) {
        await intent.Battle.WriteLineAsync(intent.ToString());
        await intent.Battle.WriteLineAsync("");
    }

    protected void FinishMapEffect(bool affected) {
        if (affected) {
            item.DeductUse();
            PlayMainSound();
        } else {
            AudioManager.PlayFail();
        }
    }

    protected virtual async Task PlayBattleAnimAsync(Intent intent) {
        if (Item.Anim == null) {
            return;
        }

        if (intent.Battle.Player.Contains(intent.Actor)) {
            var aliveTargets = new List<Unit>(intent.Targets.Where(unit => unit.IsAlive));
            await Item.Anim.PlayAsync(intent.Battle.View, aliveTargets);
        } else {
            // enemy battle animation
        }
    }
}
