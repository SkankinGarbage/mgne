using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class RecruitMenu : FullScreenMenuView {

    [SerializeField] private Text recruitSummary = null;
    [SerializeField] private ListView recruitCells = null;
    [SerializeField] private DynamicListSelector recruitSelector = null;
    [SerializeField] private Text infoText = null;
    [SerializeField] private GridSelector letterSelector = null;
    [SerializeField] private ListView letterList = null;
    [SerializeField] private UnitCellView unitCell = null;
    [SerializeField] private List<GameObject> letterHighlights = null;

    private RecruitSelectionData data;

    public static RecruitMenu ShowDefault() {
        return Instantiate<RecruitMenu>("Prefabs/UI/Recruit/RecruitMenu");
    }

    public void Populate(RecruitSelectionData data) {
        this.data = data;
        recruitSummary.text = data.title;
        var modifiedOptions = new List<CharaData>(data.options) {
            null // the "more info" cell hack
        };
        recruitCells.Populate(modifiedOptions, (obj, chara) => {
            obj.GetComponent<RecruitCellView>().Populate(chara);
        });
    }

    public async Task DoMenuAsync(RecruitSelectionData data, bool autoclose = true) {
        Populate(data);
        while (true) {
            var slot = await recruitSelector.SelectItemAsync(null, true);
            if (slot >= data.options.Length) {
                await ShowInfo();
            } else {
                var recruited = await RecruitAndNameAsync(data.options[slot]);
                if (recruited) {
                    break;
                }
            }
        }
        if (autoclose) {
            await CloseRoutine();
        }
    }

    private async Task ShowInfo() {
        infoText.gameObject.SetActive(true);
        recruitCells.gameObject.SetActive(false);
        await Global.Instance().Input.AwaitConfirm();
        infoText.gameObject.SetActive(false);
        recruitCells.gameObject.SetActive(true);
    }

    private async Task<bool> RecruitAndNameAsync(CharaData data) {
        recruitCells.gameObject.SetActive(false);
        recruitSummary.gameObject.SetActive(false);
        unitCell.gameObject.SetActive(true);
        letterList.gameObject.SetActive(true);

        var unit = new Unit(data);
        unitCell.Populate(unit);

        var chars = new List<char> { ' ' };
        for (int i = 'A'; i <= 'Z'; i += 1) {
            chars.Add((char) i);
        }
        chars.Add(' ');
        chars.Add(' ');
        for (int i = 'a'; i <= 'z'; i += 1) {
            chars.Add((char)i);
        }
        chars.Add(' ');
        chars.Add(' ');
        for (int i = '0'; i <= '9'; i += 1) {
            chars.Add((char)i);
        }
        chars.Add('-');
        chars.Add(' ');
        chars.Add(' ');
        letterList.Populate(chars, (obj, @char) => {
            obj.GetComponent<Text>().text = @char.ToString();
        });

        char[] nameArray = unit.Name.ToCharArray();
        Array.Resize(ref nameArray, 10);
        int letterIndex = 0;
        bool? result = null;
        while (!result.HasValue) {
            unit.SetName(new string(nameArray));
            unitCell.Populate(unit);
            ShowLetterHighlight(letterIndex);
            var selection = await letterSelector.SelectItemAsync(null, true);
            if (selection == GenericSelector.CodeCancel) {
                if (new string(nameArray).Trim().Length > 0) {
                    for (int i = 0; i < 10; i += 1) nameArray[i] = ' ';
                }
                if (letterIndex == 0) {
                    result = false;
                } else {
                    nameArray[letterIndex] = ' ';
                    letterIndex -= 1;
                }
            } else if (selection == GenericSelector.CodeMenu) {
                unit.SetName(new string(nameArray));
                result = true;
            } else {
                if (letterIndex == 0) {
                    for (int i = 0; i < 10; i += 1) nameArray[i] = ' ';
                }
                var cell = letterSelector.GetCell(selection);
                nameArray[letterIndex] = cell.GetComponent<Text>().text[0];
                letterIndex += 1;
            }
        }

        if (result.Value) {
            Global.Instance().Data.Party.AddMember(unit);
        } else {
            recruitCells.gameObject.SetActive(true);
            recruitSummary.gameObject.SetActive(true);
            unitCell.gameObject.SetActive(false);
            letterList.gameObject.SetActive(false);
        }
        return result.Value;
    }

    private void ShowLetterHighlight(int index) {
        foreach (var highlight in letterHighlights) {
            highlight.SetActive(false);
        }
        if (index >= 0) {
            letterHighlights[index].SetActive(true);
        }
    }
}
