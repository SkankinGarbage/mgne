using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DollArea : MonoBehaviour {

    [SerializeField] private List<EnemyDollSprite> availableCells = null;
    [SerializeField] private Camera cam = null;

    private Dictionary<Unit, EnemyDollSprite> sprites = new Dictionary<Unit, EnemyDollSprite>();
    private Dictionary<Unit, float> positionRatios = new Dictionary<Unit, float>();

    public Sprite Populate(IEnumerable<Unit> enemyGroup, float positionRatio) {
        var enemy = enemyGroup.First();
        EnemyDollSprite sprite = null;
        if (!sprites.ContainsKey(enemy)) {
            var doll = availableCells[0];
            availableCells.RemoveAt(0);
            doll.transform.SetParent(transform);
            sprite = doll.GetComponent<EnemyDollSprite>();
            sprites[enemy] = sprite;
        } else {
            sprite = sprites[enemy];
        }
        positionRatios[enemy] = positionRatio;
        return sprite.Populate(enemyGroup);
    }

    public Vector3 OffsetForEnemy(IEnumerable<Unit> enemyGroup) {
        var enemy = enemyGroup.First();
        return sprites[enemy].transform.localPosition;
    }

    public void LateUpdate() {
        foreach (var dollSpritePair in sprites) {
            var ui = dollSpritePair.Key;
            var oldPos = dollSpritePair.Value.transform.position;
            var width = cam.aspect * cam.orthographicSize * 2;
            dollSpritePair.Value.transform.position = new Vector3(
                width * positionRatios[ui] - width / 2,
                oldPos.y,
                oldPos.z);
        }
    }
}
