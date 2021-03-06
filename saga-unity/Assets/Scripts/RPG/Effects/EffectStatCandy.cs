﻿using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EffectStatCandy : AbilEffect {

    protected new EffectStatCandyData data;

    public EffectStatCandy(EffectStatCandyData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => false;
    public override bool IsMapUsable() => true;

    public override async Task<bool> UseOnMapAsync(IItemUseableMenu menu, Unit user) {
        var affected = false;
        var wasActive = menu.IsActive();

        menu.SetActive(true);
        var target = await menu.SelectUnitTargetAsync();

        if (target != null) {
            if (data.restrictRace.Where(x => x == target.Race).Count() == 0) {
                affected = false;
            } else if (data.maxValue != 0 && target.BaseStats[data.stat] > data.maxValue) {
                affected = false;
                // TODO: polish: maybe print an error message?
            } else {
                affected = true;
                int delta = data.maxGain - data.minGain;
                if (delta > 0) delta = Random.Range(0, delta);
                target.PermanentlyModifyStat(data.stat, data.minGain + delta);
            }
            FinishMapEffect(affected);
        }
        if (affected) {
            menu.Repopulate();
            await menu.ConfirmAsync();
            menu.SetActive(wasActive);
        }
        return affected;
    }
}