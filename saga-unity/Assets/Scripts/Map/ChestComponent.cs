using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using System.Collections;
using UnityEngine;

public class ChestComponent : MonoBehaviour {

    private const string PropertyItem = "item";
    private const string PropertyKeyItem = "keyItem";
    private const string PropertyVisible = "visible";

    [SerializeField] private Sprite closedSprite = null;
    [SerializeField] private Sprite openSprite = null;
    [SerializeField] private string sfxKey = "get";
    [Space]
    [SerializeField] private MapEvent @event = null;
    [SerializeField] private new SpriteRenderer renderer = null;
    [SerializeField] private string itemKey = null;
    [SerializeField] private string encounterKey = null;
    [SerializeField] private bool isInivisble = false;
    [SerializeField] private bool isKey = false;

    private string switchName;
    private string SwitchName {
        get {
            if (switchName == null) {
                switchName = @event.Map.MapName + ":" + @event.Position.x + ":" + @event.Position.y;
            }
            return switchName;
        }
    }

    public void Reconfigure(MapEvent parent, TmxAssetImporter importer) {
        @event = GetComponentInParent<MapEvent>();

        var props = parent.Properties;
        CustomProperty prop;
        if (props.TryGetCustomProperty(PropertyItem, out prop)) {
            itemKey = prop.GetValueAsString();
        }
        if (props.TryGetCustomProperty(PropertyVisible, out prop)) {
            isInivisble = prop.GetValueAsString() != "VISIBLE";
        }
        if (props.TryGetCustomProperty(PropertyKeyItem, out prop)) {
            isKey = true;
            itemKey = prop.GetValueAsString();
        }
    }

    public void Start() {
        @event.GetComponent<Dispatch>().RegisterListener(MapEvent.EventInteract, (object payload) => {
            OnInteract((AvatarEvent)payload);
        });
        @event.ImpassabilityOverride = true;
        UpdateSprite();
    }

    private bool IsOpen() {
        if (isKey) {
            var item = IndexDatabase.Instance().CombatItems.GetData(itemKey);
            return !Global.Instance().Data.Inventory.ContainsItemType(item);
        } else {
            return Global.Instance().Data.GetSwitch(SwitchName);
        }
    }

    private void UpdateSprite() {
        renderer.sprite = IsOpen() ? openSprite : (isInivisble ? null : closedSprite);
    }

    private void OnInteract(AvatarEvent avatar) {
        if (!IsOpen() && @event.IsSwitchEnabled) {
            StartCoroutine(OpenRoutine());
        }
    }

    private IEnumerator OpenRoutine() {
        Global.Instance().Data.SetSwitch(SwitchName, true);
        UpdateSprite();

        var textbox = MapOverlayUI.Instance().textbox;
        if (encounterKey != null && encounterKey.Length > 0) {
            var party = IndexDatabase.Instance().Parties.GetData(encounterKey);
            yield return BattleView.SpawnBattleRoutine(party);
        } else if (itemKey != null && itemKey.Length > 0) {
            var item = new CombatItem(IndexDatabase.Instance().CombatItems.GetData(itemKey));
            if (Global.Instance().Data.Inventory.IsFull()) {
                yield return textbox.SpeakRoutine("No room for more items.");
            } else {
                Global.Instance().Audio.PlaySFX(sfxKey);
                if (isKey) {
                    yield return textbox.SpeakRoutine("Retrieved the " + item.Name + ".");
                } else {
                    yield return textbox.SpeakRoutine("Retrieved a " + item.Name + ".");
                }
                Global.Instance().Data.Inventory.Add(item);
            }
            yield return textbox.DisableRoutine();
        } else {
            yield return textbox.SpeakRoutine("Empty.");
            yield return textbox.DisableRoutine();
        }
    }
}
