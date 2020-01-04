[UnityEngine.CreateAssetMenu(fileName="NumberSet", menuName="Data/Ui/")]
public class NumberSetData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Graphics file")]
    public string file;
    
    [UnityEngine.Tooltip("Width - Width of a number, in px")]
    public int width;
    
    [UnityEngine.Tooltip("Height - Height of a number, in px")]
    public int height;
    
    [UnityEngine.Tooltip("Kerning - Space between two numbers when printed, in px")]
    public int kerning;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
