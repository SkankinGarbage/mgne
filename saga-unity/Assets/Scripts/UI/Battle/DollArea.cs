using UnityEngine;
using System.Collections.Generic;

public class DollArea : MonoBehaviour {

    [SerializeField] private List<EnemyDollSprite> availableCells;
    [SerializeField] private Camera cam;

    private Dictionary<EnemyDoll, EnemyDollSprite> sprites = new Dictionary<EnemyDoll, EnemyDollSprite>();
    private Dictionary<EnemyDoll, float> positionRatios = new Dictionary<EnemyDoll, float>();

    public Sprite Populate(EnemyDoll ui, IEnumerable<Unit> enemyGroup, float positionRatio) {
        EnemyDollSprite sprite = null;
        if (!sprites.ContainsKey(ui)) {
            var doll = availableCells[0];
            availableCells.RemoveAt(0);
            doll.transform.SetParent(transform);
            sprite = doll.GetComponent<EnemyDollSprite>();
            sprites[ui] = sprite;
        } else {
            sprite = sprites[ui];
        }
        positionRatios[ui] = positionRatio;
        return sprite.Populate(enemyGroup);
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
