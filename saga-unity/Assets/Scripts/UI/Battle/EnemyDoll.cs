using UnityEngine;
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
        var path = PortraitPath + portrait;
        return Resources.Load<Sprite>(path);
    }
}
