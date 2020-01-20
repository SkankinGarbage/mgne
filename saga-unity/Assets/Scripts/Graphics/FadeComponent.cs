﻿using UnityEngine;
using System.Collections;
using System;

public abstract class FadeComponent : MonoBehaviour {

    public IEnumerator TransitionRoutine(TransitionData transition, Action intermediate = null) {
        yield return StartCoroutine(FadeRoutine(transition.GetFadeOut()));
        intermediate?.Invoke();
        yield return StartCoroutine(FadeRoutine(transition.GetFadeIn(), true));
    }

    public abstract IEnumerator FadeRoutine(FadeData fade, bool invert = false, float timeMult = 1.0f);
}
