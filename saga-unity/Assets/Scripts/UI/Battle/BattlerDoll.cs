using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class BattlerDoll : MonoBehaviour {

    [SerializeField] public FieldSpriteImage image;
    [Space]
    [SerializeField] public float damageDuration = 0.4f;
    [SerializeField] public int maxDisplacement = 4;

    public Unit Unit { get; private set; }

    public void Populate(Unit unit) {
        Unit = unit;
        image.Populate(unit.FieldSpriteTag);
        image.animates = unit.IsAlive;
    }

    public IEnumerator TakeDamageRoutine(int damage) {
        if (damage > 0) {
            var transform = image.GetComponent<RectTransform>();
            var elapsed = 0.0f;
            var originalPosition = transform.anchoredPosition;
            var even = false;
            while (elapsed < damageDuration) {
                elapsed += Time.deltaTime;
                var offset = Random.Range(1, maxDisplacement);
                if (even) offset *= -1;
                even = !even;
                transform.anchoredPosition = new Vector2(
                    transform.anchoredPosition.x,
                    originalPosition.y + offset);
                yield return null;
            }
            transform.anchoredPosition = originalPosition;
        }
    }
}
