using UnityEngine;
using UnityEditor;

[UnityEngine.CreateAssetMenu(fileName = "GameboyMaterialSettings", menuName = "Data/Graphics/GameboyMaterialSettings")]
public class GameboyMaterialSettings : ScriptableObject {

    private const string DefaultPath = "Assets/Resources/Materials/Gameboy/GameboyMaterialSettings.asset";

    public Material BackgroundMaterial;
    public Material ForegroundMaterial;

#if UNITY_EDITOR

    public static GameboyMaterialSettings GetDefault() {
        return AssetDatabase.LoadAssetAtPath<GameboyMaterialSettings>(DefaultPath);
    }

#endif
}
