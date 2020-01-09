using UnityEditor;

[CustomEditor(typeof(CombatItemData))]
public class CombatItemEditor : Editor {

    private PolymorphicFieldUtility warheadUtil;

    public void OnEnable() {
        CombatItemData combatItem = (CombatItemData)target;
        warheadUtil = new PolymorphicFieldUtility(typeof(AbilEffectData), typeof(CombatItemData), combatItem.warhead);
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        CombatItemData combatItem = (CombatItemData)target;

        if (!serializedObject.FindProperty("warhead").hasMultipleDifferentValues) {
            combatItem.warhead = warheadUtil.DrawSelector(combatItem.warhead);
        }
    }
}
