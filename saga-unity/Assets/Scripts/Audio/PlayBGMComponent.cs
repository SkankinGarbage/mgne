using UnityEngine;

public class PlayBGMComponent : MonoBehaviour {

    [SerializeField] private string bgmKey;

    public void OnEnable() {
        Global.Instance().Audio.PlayBGM(bgmKey);
    }
}
