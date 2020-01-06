[UnityEngine.CreateAssetMenu(fileName="Palette", menuName="Data/Graphics/")]
public class PaletteData : MainSchema {

    [UnityEngine.Tooltip("Output black - output color shaders use as the gameboy black, use rrr,ggg,bbb for" +
"mat")]
    public string outBlack;

    [UnityEngine.Tooltip("Output dark gray - output color shaders use as the gameboy dark gray, use rrr,ggg" +
",bbb format")]
    public string outDgray;

    [UnityEngine.Tooltip("Output light gray - output color shaders use as the gameboy light gray, use rrr,g" +
"gg,bbb format")]
    public string outLgray;

    [UnityEngine.Tooltip("Output white - output color shaders use as the gameboy white, use rrr,ggg,bbb for" +
"mat")]
    public string outWhite;
}
