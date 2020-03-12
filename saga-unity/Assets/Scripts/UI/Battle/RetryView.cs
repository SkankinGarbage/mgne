using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryView : MonoBehaviour {

    [SerializeField] private ExpanderComponent expander = null;
    [SerializeField] private GenericSelector choiceSelector = null;
    [SerializeField] private TextAutotyper type = null;

    private FadeComponent fade;
    private FadeComponent Fade {
        get {
            if (fade == null) {
                fade = FindObjectOfType<FadeComponent>();
            }
            return fade;
        }
    }

    public async Task<bool> RetryAsync(Battle battle) {
        var text = type.textbox.text;
        type.textbox.text = "";
        gameObject.SetActive(true);
        await fade.FadeInRoutine();
        await type.TypeRoutine(text, false);
        await expander.ShowRoutine();
        while (true) {
            var command = await choiceSelector.SelectCommandAsync();
            switch (command) {
                case "end":
                    await fade.FadeOutRoutine("black");
                    SceneManager.LoadScene("Title");
                    return false;
                case "retry":
                    await fade.FadeOutRoutine("white");
                    battle.Reset();
                    gameObject.SetActive(false);
                    await fade.FadeInRoutine("white");
                    return true;
            }
        }
    }
}
