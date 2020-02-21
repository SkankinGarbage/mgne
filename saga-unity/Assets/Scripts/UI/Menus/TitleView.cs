using UnityEngine;
using System.Threading.Tasks;

public class TitleView : FullScreenMenuView {

    [SerializeField] private ListSelector mainMenu = null;

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
                    SelectStart();
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

    private void SelectStart() {
        
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
}
