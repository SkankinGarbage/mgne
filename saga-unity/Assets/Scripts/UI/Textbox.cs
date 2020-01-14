using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Textbox : MonoBehaviour, InputListener {

    private static readonly string SystemSpeaker = "SYSTEM";
    private static float OverlapSlop = 5f;

    [Header("Config")]
    public float charsPerSecond = 120f;
    public float animationSeconds = 0.2f;
    public float textClearSeconds = 0.1f;

    [Space]
    [Header("Hookups")]
    public Text namebox;
    public Text textbox;
    public RectTransform mainBox;
    public GameObject advanceArrow;

    [Space]
    [Header("Sizing")]
    public float minTextHeight = 12;
    [HideInInspector] [SerializeField] protected Vector2 textMaxSize;
    [HideInInspector] [SerializeField] protected float nameboxHeight;

    public bool isDisplaying { get; private set; }
    
    private bool hurried;
    private bool confirmed;

    public void Start() {
        textbox.text = "";
        mainBox.sizeDelta = new Vector2(mainBox.sizeDelta.x, 0.0f);
        advanceArrow.SetActive(false);

        StartCoroutine(TestRoutine());
    }

    public void MemorizeSizes() {
        textMaxSize = mainBox.sizeDelta;
        nameboxHeight = namebox.GetComponent<RectTransform>().sizeDelta.y;
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        switch (eventType) {
            case InputManager.Event.Down:
                if (command == InputManager.Command.Confirm) {
                    hurried = true;
                }
                break;
            case InputManager.Event.Up:
                if (command == InputManager.Command.Confirm) {
                    confirmed = true;
                }
                break;
        }
        return true;
    }

    public IEnumerator TestRoutine() {
        yield return CoUtils.Wait(2.0f);
        isDisplaying = true;
        while (true) {
            yield return DisableRoutine();
            yield return CoUtils.Wait(1.0f);
            yield return SpeakRoutine("Diaghilev", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
                "eiusmod tempor incididunt ut labore et dolore magna aliqua.");
            yield return SpeakRoutine("Diaghilev", "Etc.");
            yield return SpeakRoutine("Homasa", "Hello I am someone completely different");
        }
    }

    public IEnumerator SpeakRoutine(string text) {
        yield return SpeakRoutine(SystemSpeaker, text);
    }

    public IEnumerator SpeakRoutine(string speakerName, string text) {
        if (text == null || text.Length == 0) {
            text = speakerName;
            speakerName = null;
        }
        if (!isDisplaying) {
            namebox.enabled = speakerName != SystemSpeaker;
            namebox.text = speakerName;
            yield return EnableRoutine();
        } else {
            yield return EraseTextRoutine(textClearSeconds);
            if (namebox.text != speakerName) {
                yield return NameboxSpeakerSwitchStartRoutine(animationSeconds);
                namebox.enabled = speakerName != SystemSpeaker;
                namebox.text = speakerName;
                yield return NameboxSpeakerSwitchEndRoutine(animationSeconds);
            }
        }

        yield return TypeRoutine(text);
    }

    public virtual void Hide() {
        mainBox.anchoredPosition = new Vector3(mainBox.anchoredPosition.x, -minTextHeight * 2 + OverlapSlop);
        namebox.rectTransform.sizeDelta = new Vector2(mainBox.sizeDelta.x, 0.0f);
        mainBox.sizeDelta = new Vector2(mainBox.sizeDelta.x, minTextHeight);
    }

    public virtual void Show() {
        mainBox.anchoredPosition = new Vector3(mainBox.anchoredPosition.x, 0);
        namebox.rectTransform.sizeDelta = new Vector2(mainBox.sizeDelta.x, nameboxHeight);
        mainBox.sizeDelta = new Vector2(textMaxSize.x, textMaxSize.y);
    }

    public IEnumerator DisableRoutine() {
        isDisplaying = false;
        yield return CoUtils.RunParallel(new IEnumerator[] {
            EraseTextRoutine(animationSeconds / 2.0f),
            HideNameBoxRoutine(animationSeconds),
            CloseMainBoxRoutine(animationSeconds),
        }, this);
        Global.Instance().Input.RemoveListener(this);
    }

    private IEnumerator EnableRoutine() {
        isDisplaying = true;
        Global.Instance().Input.PushListener(this);

        yield return CoUtils.RunParallel(new IEnumerator[] {
            ShowNameBoxRoutine(animationSeconds),
            ShowMainBoxRoutine(animationSeconds),
        }, this);
    }

    protected virtual IEnumerator ShowNameBoxRoutine(float seconds) {
        yield return CoUtils.RunTween(namebox.GetComponent<RectTransform>().DOSizeDelta(new Vector2(mainBox.sizeDelta.x, nameboxHeight), seconds));
    }
    protected virtual IEnumerator HideNameBoxRoutine(float seconds) {
        yield return CoUtils.RunTween(namebox.rectTransform.DOSizeDelta(new Vector2(namebox.rectTransform.sizeDelta.x, 0.0f), seconds));
    }

    protected virtual IEnumerator EraseTextRoutine(float seconds) {
        yield return CoUtils.RunTween(textbox.GetComponent<CanvasGroup>().DOFade(0.0f, seconds));
    }
    protected virtual IEnumerator EraseNameRoutine(float seconds) {
        yield return CoUtils.RunTween(namebox.GetComponent<CanvasGroup>().DOFade(0.0f, seconds));
    }

    protected virtual IEnumerator ShowMainBoxRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
            CoUtils.RunTween(mainBox.DOSizeDelta(new Vector2(mainBox.sizeDelta.x, textMaxSize.y), seconds)),
            CoUtils.RunTween(mainBox.DOLocalMoveY(0.0f, seconds, true)),
        }, this);
    }
    protected virtual IEnumerator CloseMainBoxRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
            CoUtils.RunTween(mainBox.DOSizeDelta(new Vector2(mainBox.sizeDelta.x, minTextHeight), seconds)),
            CoUtils.RunTween(mainBox.DOLocalMoveY(-minTextHeight * 2 + OverlapSlop, seconds, true)),
        }, this);
    }

    protected virtual IEnumerator NameboxSpeakerSwitchStartRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
                    CloseMainBoxRoutine(seconds),
                    HideNameBoxRoutine(seconds),
                }, this);
    }
    protected virtual IEnumerator NameboxSpeakerSwitchEndRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
                    ShowMainBoxRoutine(animationSeconds),
                    ShowNameBoxRoutine(animationSeconds),
                }, this);
    }

    private IEnumerator TypeRoutine(string text) {
        hurried = false;
        float elapsed = 0.0f;
        float total = text.Length / charsPerSecond;
        textbox.GetComponent<CanvasGroup>().alpha = 1.0f;
        while (elapsed <= total) {
            elapsed += Time.deltaTime;
            int charsToShow = Mathf.FloorToInt(elapsed * charsPerSecond);
            int cutoff = charsToShow > text.Length ? text.Length : charsToShow;
            textbox.text = text.Substring(0, cutoff);
            textbox.text += "<color=#00000000>";
            textbox.text += text.Substring(cutoff);
            textbox.text += "</color>";
            yield return null;

            if (hurried) {
                break;
            }
        }
        textbox.text = text;

        confirmed = false;
        advanceArrow.SetActive(true);
        while (!confirmed) {
            yield return null;
        }
        advanceArrow.SetActive(false);
    }
}
