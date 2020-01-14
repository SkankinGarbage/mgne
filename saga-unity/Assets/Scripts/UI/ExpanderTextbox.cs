﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ExpanderTextbox : Textbox {

    [SerializeField] private int StepSize = 16;

    public override void Hide() {
        namebox.transform.parent.gameObject.SetActive(false);
        mainBox.gameObject.SetActive(false);
        mainBox.sizeDelta = new Vector2(minTextHeight, minTextHeight);
    }
    public override void Show() {
        namebox.transform.parent.gameObject.SetActive(true);
        mainBox.gameObject.SetActive(true);
        mainBox.sizeDelta = new Vector2(textMaxSize.x, textMaxSize.y);
    }

    protected override IEnumerator ShowNameBoxRoutine(float seconds) {
        yield return CoUtils.Wait(seconds);
        namebox.transform.parent.gameObject.SetActive(true);
    }
    protected override IEnumerator HideNameBoxRoutine(float seconds) {
        namebox.transform.parent.gameObject.SetActive(false);
        yield return CoUtils.Wait(seconds);
    }

    protected override IEnumerator ShowMainBoxRoutine(float seconds) {
        mainBox.gameObject.SetActive(true);
        yield return CoUtils.StepResize2(x => mainBox.sizeDelta = x, mainBox.sizeDelta, new Vector2(textMaxSize.x, textMaxSize.y), StepSize, seconds);
    }
    protected override IEnumerator CloseMainBoxRoutine(float seconds) {
        yield return EraseTextRoutine(seconds / 2f);
        yield return CoUtils.StepResize2(x => mainBox.sizeDelta = x, mainBox.sizeDelta, new Vector2(minTextHeight, minTextHeight), StepSize, seconds);
        mainBox.gameObject.SetActive(false);
    }

    protected override IEnumerator NameboxSpeakerSwitchStartRoutine(float seconds) {
        namebox.transform.parent.gameObject.SetActive(false);
        yield return CoUtils.Wait(seconds / 12f);
    }
    protected override IEnumerator NameboxSpeakerSwitchEndRoutine(float seconds) {
        namebox.transform.parent.gameObject.SetActive(true);
        yield return null;
    }

    protected override IEnumerator EraseTextRoutine(float seconds) {
        yield return CoUtils.Wait(seconds / 2f);
        textbox.GetComponent<CanvasGroup>().alpha = 0.5f;
        yield return CoUtils.Wait(seconds / 2f);
        textbox.GetComponent<CanvasGroup>().alpha = 0.0f;
    }
}
