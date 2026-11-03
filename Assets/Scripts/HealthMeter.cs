using UnityEngine;

public class HealthMeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _healthMarker;

    private float _healthBarInterval;

    private void Awake()
    {
        _healthBarInterval = GetComponent<RectTransform>().sizeDelta.x / (GameManager.MAX_HEALTH * 2);
        UpdateHealthMarker();
    }

    public void UpdateHealthMarker()
    {
        Vector3 position = _healthMarker.localPosition;
        position.x = GameManager.Instance.currentWinStateClamped * _healthBarInterval;
        _healthMarker.localPosition = position;
    }
}
