using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class FadeMaterialComponent : FadeComponent {

    private List<Material> materialsToFade;

    public override IEnumerator FadeRoutine(FadeData fade, bool invert = false, float timeMult = 1) {
        float elapsed = 0.0f;
        float transitionDuration = fade.delay * timeMult;

        materialsToFade = new List<Material>() {
            Global.Instance().Maps.avatar.Chara.Doll.Renderer.sharedMaterial,
        };
        foreach (var layer in Global.Instance().Maps.activeMap.layers) {
            materialsToFade.Add(layer.GetComponent<TilemapRenderer>()?.sharedMaterial);
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
    }
}
