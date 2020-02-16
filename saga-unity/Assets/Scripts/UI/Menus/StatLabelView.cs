using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatLabelView : MonoBehaviour {

    [SerializeField] private StatTag stat = StatTag.HP;
    [SerializeField] private Text label = null;
    [SerializeField] private Text value = null;

    public void Populate(StatSet stats) {
        var tag = Stat.Get(stat).NameShort;
        if (tag.Length < 4) tag += ":";
        label.text = tag;
        value.text = Mathf.FloorToInt(stats[stat]).ToString();
    }
}
