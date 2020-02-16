using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyNameCellView : MonoBehaviour {

    [SerializeField] private Text enemyName = null;
    [SerializeField] private Text enemyNumber = null;

    public void Populate(IEnumerable<Unit> enemies) {
        enemyName.text = enemies.First().Name;
        int alive = enemies.Where(enemy => !enemy.IsDead).Count();
        enemyNumber.text = "x" + alive;
    }

    public override string ToString() {
        return base.ToString() + " " + enemyName.text + " " + enemyNumber.text;
    }
}
