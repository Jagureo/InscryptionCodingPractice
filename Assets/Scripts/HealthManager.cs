using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthMeter _healthMeter;
    [SerializeField] private HealthScale _healthScale;

    public void UpdateHealthDisplay()
    {
        _healthMeter.UpdateHealthMarker();
        _healthScale.UpdateScaleDisplay();
    }
}
