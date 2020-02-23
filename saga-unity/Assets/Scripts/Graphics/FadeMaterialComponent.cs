using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FadeMaterialComponent : FadeComponent {

    [SerializeField] private List<SpriteRenderer> renderersToFade = null;
    [SerializeField] private List<Image> imagesToFade = null;
    [SerializeField] private List<Text> textsToFade = null;

    private List<Material> materialsToFade;

    public override IEnumerator FadeRoutine(FadeData fade, bool invert = false, float timeMult = 1) {
        float elapsed = 0.0f;
        float transitionDuration = fade.delay * timeMult;

        materialsToFade = new List<Material>();
        if (Global.Instance().Maps.ActiveMap != null) {
            materialsToFade.Add(Global.Instance().Maps.Avatar.Chara.Doll.Renderer.sharedMaterial);
            foreach (var layer in Global.Instance().Maps.ActiveMap.layers) {
                materialsToFade.Add(layer.GetComponent<TilemapRenderer>()?.sharedMaterial);
            }
        }
        foreach (var renderer in renderersToFade) {
            materialsToFade.Add(renderer.sharedMaterial);
        }
        foreach (var image in imagesToFade) {
            materialsToFade.Add(image.material);
        }
        foreach (var text in textsToFade) {
            materialsToFade.Add(text.material);
        }

        while (elapsed < transitionDuration) {
            elapsed += Time.fixedDeltaTime;
            foreach (var material in materialsToFade) {
                AssignShaderValues(material, fade, invert, elapsed / transitionDuration);
            }
            yield return null;
        }
        foreach (var material in materialsToFade) {
            AssignShaderValues(material, fade, invert, 1.0f);
        }
    }

    private void AssignShaderValues(Material material, FadeData fade, bool invert, float t) {
        material.SetFloat("_FadeOffset", invert ? (1.0f - t) : t);
        material.SetInt("_Invert", fade.invert ? 1 : 0);
        material.SetFloat("_FadeColorMod", fade.brightnessMod);
    }
}
