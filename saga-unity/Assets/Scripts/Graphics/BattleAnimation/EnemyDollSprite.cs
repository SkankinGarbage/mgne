using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyDollSprite : MonoBehaviour {

    private const string PortraitPath = "Sprites/Battlers/";

    [SerializeField] private new SpriteRenderer renderer = null;
    [SerializeField] private Sprite nullSprite = null;

    private string oldSpriteName;

    public Sprite Populate(IEnumerable<Unit> enemyGroup) {
        if (enemyGroup.Any(unit => unit.IsAlive)) {
            var enemy = enemyGroup.First();
            if (enemy.Portrait != oldSpriteName) {
                renderer.sprite = LoadSpriteFromPortraitName(enemy.Portrait);
                oldSpriteName = enemy.Portrait;
            }
        } else {
            renderer.sprite = nullSprite;
        }
        return renderer.sprite;
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
