using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(SelectableCell))]
public class EnemyDoll : MonoBehaviour {

    [SerializeField] private Image invisibleImage = null;

    public void Populate(IEnumerable<Unit> enemyGroup, DollArea dollArea, float positionRatio) {
        GetComponent<SelectableCell>().SetSelectable(enemyGroup.Any(enemy => !enemy.IsDead));
        var sprite = dollArea.Populate(this, enemyGroup, positionRatio);
        invisibleImage.sprite = sprite;
        invisibleImage.SetNativeSize();
        invisibleImage.color = new Color(1, 1, 1, 0);
    }
}
