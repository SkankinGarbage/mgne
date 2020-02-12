﻿using UnityEngine;
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

    public virtual void OnMapUse(UnitList menu, Unit user) {
        Debug.LogError("Unimplemented map use for item " + this);
    }

    public void PlayMainSound() {
        // TODO: extract audio from battle anim
    }

    protected void FinishMapEffect(bool affected) {
        if (affected) {
            item.DeductUse();
            PlayMainSound();
        } else {
            AudioManager.PlayFail();
        }
    }
}