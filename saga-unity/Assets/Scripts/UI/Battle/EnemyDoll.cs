using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SelectableCell))]
public class EnemyDoll : MonoBehaviour {

    public void Populate(IEnumerable<Unit> enemyGroup, DollArea dollArea, float positionRatio) {
        GetComponent<SelectableCell>().SetSelectable(enemyGroup.Any(enemy => !enemy.IsDead));
        var sprite = dollArea.Populate(enemyGroup, positionRatio);
        var trans = GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(positionRatio * 320 - 160, trans.anchoredPosition.y);
        trans.sizeDelta = sprite.rect.size;
    }
}
