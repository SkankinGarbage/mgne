using UnityEngine;
using System.Threading.Tasks;

public class TitleView : FullScreenMenuView {

    [SerializeField] private ListSelector mainMenu = null;
    [SerializeField] private TextScrollView scrollView = null;
    [Space]
    [SerializeField] private RecruitSelectionData recruitLeader = null;
    [SerializeField] private RecruitSelectionData recruitFollower = null;
    [SerializeField] private string defaultMapKey = "world1/parish";
    [SerializeField] private string defaultMapTarget = "start";

    private FadeComponent fade;

    public void OnEnable() {
        MenuAync();
        fade = FindObjectOfType<FadeComponent>();
    }

    public async void MenuAync() {
        if (Global.Instance().Serialization.DoSavedGamesExist()) {
            mainMenu.Selection = 1; // CONTINUE
        }
        while (true) {
            var task = mainMenu.SelectCommandAsync(null, true);
            var command = await task;
            switch (command) {
                case "START":
                    await SelectStart();
                    return;
                case "CONTINUE":
                    bool continued = await SelectContinue();
                    if (continued) {
                        return;
                    }
                    break;
                case "OPTIONS":
                    await SelectOptions();
                    break;
            }
        }
    }

    private async Task SelectStart() {
        await fade.FadeOutRoutine("white");
        scrollView.gameObject.SetActive(true);
        await fade.FadeInRoutine("white");
        await scrollView.ScrollRoutine();

        await fade.FadeOutRoutine("black");
        await RecruitAsync(recruitLeader);
        await RecruitAsync(recruitFollower);
        await RecruitAsync(recruitFollower);
        await RecruitAsync(recruitFollower);

        await Task.Delay(1000);
        await Global.Instance().Maps.TeleportRoutine(defaultMapKey, defaultMapTarget, OrthoDir.South, true);
        await fade.FadeInRoutine("black_long");
    }

    private async Task<bool> SelectContinue() {
        var saveMenu = SaveMenuView.ShowDefault();
        var loaded = await saveMenu.DoMenuAsync(SaveMenuView.Mode.Load);
        if (loaded > -1) {
            await fade.FadeOutRoutine();
            await saveMenu.CloseRoutine();
            Global.Instance().StartCoroutine(Global.Instance().Serialization.LoadGameRoutine(loaded));
            return true;
        }
        return false;
    }

    private async Task SelectOptions() {
        // TODO
        await Task.Delay(100);
    }

    private async Task RecruitAsync(RecruitSelectionData data) {
        var menu = RecruitMenu.ShowDefault();
        var recruitTask = menu.DoMenuAsync(data, autoclose: false);
        await fade.FadeInRoutine("black");
        await recruitTask;
        await fade.FadeOutRoutine("black");
        await menu.CloseRoutine();
    }
}
