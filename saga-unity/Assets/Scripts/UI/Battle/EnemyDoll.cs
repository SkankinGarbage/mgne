﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class EnemyDoll : MonoBehaviour {

    private const string PortraitPath = "Sprites/Battlers/";

    public Image image;

    public void Populate(IEnumerable<Unit> enemyGroup) {
        if (enemyGroup.Any(enemy => !enemy.IsDead)) {
            var enemy = enemyGroup.First();
            image.sprite = LoadSpriteFromPortraitName(enemy.Portrait);
        } else {
            image.sprite = null;
        }
        image.SetNativeSize();
    }

    public Sprite LoadSpriteFromPortraitName(string portrait) {
        var index = portrait.LastIndexOf('/');
        string tag;
        if (index > 0) {
            tag = portrait.Substring(index + 1, portrait.Length - index - ".png".Length - 1);
        } else {
            tag = portrait;
        }
        var path = PortraitPath + tag;
        return Resources.Load<Sprite>(path);
    }
}