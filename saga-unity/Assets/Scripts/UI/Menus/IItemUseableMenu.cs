using System.Threading.Tasks;

public interface IItemUseableMenu {

    Task<Unit> SelectUnitTargetAsync();

    Task<bool> ConfirmSelectionAsync();

    void SelectAll();

    bool IsActive();

    void SetActive(bool active);
}
