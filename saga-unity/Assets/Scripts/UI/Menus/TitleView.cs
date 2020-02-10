using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class TitleView : FullScreenMenuView {

    [SerializeField] private ListSelector mainMenu = null;

    public void OnEnable() {
        MenuAync();
    }


    public async void MenuAync() {
        while (true) {
            var task = mainMenu.SelectCommandAsync();
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
        // TODO
        await Task.Delay(100);
        return false;
    }

    private async Task SelectOptions() {
        // TODO
        await Task.Delay(100);
    }
}
