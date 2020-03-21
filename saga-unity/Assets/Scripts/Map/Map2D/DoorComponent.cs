using SuperTiled2Unity;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MapEvent))]
public class DoorComponent : MonoBehaviour {

    private const string PropertyKeyItem = "keyItem";

    [SerializeField] private string itemKey = null;

    private MapEvent @event;
    public MapEvent Event {
        get {
            if (@event == null) {
                @event = GetComponent<MapEvent>();
            }
            return @event;
        }
    }

    private string switchName;
    private string SwitchName {
        get {
            if (switchName == null) {
                switchName = Event.Map.MapName + ":" + Event.Position.x + ":" + Event.Position.y;
            }
            return switchName;
        }
    }

    private bool IsOpen => Global.Instance().Data.GetSwitch(SwitchName);

    #if UNITY_EDITOR

    public void Reconfigure(MapEvent parent, SuperTiled2Unity.Editor.TmxAssetImporter importer) {
        var props = parent.Properties;
        if (props.TryGetCustomProperty(PropertyKeyItem, out CustomProperty prop)) {
            itemKey = prop.GetValueAsString();
        }
    }

    #endif

    public void OnEnable() {
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventInteract, OnInteract);
        if (IsOpen) {
            Event.IsSwitchEnabled = false;
        }
    }

    public void OnDisable() {
        GetComponent<Dispatch>().UnregisterListener(MapEvent.EventInteract, OnInteract);
    }

    private void OnInteract(object sender) {
        if (!IsOpen) {
            var data = IndexDatabase.Instance().CombatItems.GetData(itemKey);
            if (Global.Instance().Data.Inventory.ContainsItemType(data)) {
                StartCoroutine(OpenRoutine());
            } else {
                StartCoroutine(ClosedRoutine());
            }
        }
    }

    private IEnumerator OpenRoutine() {
        var data = IndexDatabase.Instance().CombatItems.GetData(itemKey);
        var textbox = MapOverlayUI.Instance().textbox;
        Global.Instance().Data.SetSwitch(SwitchName, true);
        yield return textbox.SpeakRoutine("Used the " + UIUtils.GlyphifyString(data.abilityName) + ".");
        yield return textbox.DisableRoutine();
        Event.IsSwitchEnabled = false;
    }

    private IEnumerator ClosedRoutine() {
        var textbox = MapOverlayUI.Instance().textbox;
        yield return textbox.SpeakRoutine("Locked.");
        yield return textbox.DisableRoutine();
    }
}
