using UnityEngine;
using System.Collections;
using UnityEditor;

[UnityEngine.CreateAssetMenu(fileName = "GameboyMaterialSettings", menuName = "Data/Graphics/GameboyMaterialSettings")]
public class GameboyMaterialSettings : ScriptableObject {

    private const string DefaultPath = "Assets/Resources/Materials/Gameboy/GameboyMaterialSettings.asset";

    public Material BackgroundMaterial;
    public Material ForegroundMaterial;

    public static GameboyMaterialSettings GetDefault() {
        return AssetDatabase.LoadAssetAtPath<GameboyMaterialSettings>(DefaultPath);
    }
}
