using UnityEngine;
using UnityEngine.UI;

public class HealthScale : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _scaleRod;
    [SerializeField] private Transform _scaleLeft;
    [SerializeField] private Transform _scaleRight;
    [SerializeField] private Image _scaleLeftValue;
    [SerializeField] private Image _scaleRightValue;


    [Header("Art Assets")]
    [SerializeField] private Sprite[] _amountCount;

    private const float MAX_TILT = 15;
    private float _tiltInterval;

    private void Awake()
    {
        _tiltInterval = MAX_TILT / (GameManager.MAX_HEALTH * 2);
        UpdateScaleDisplay();
    }

    public void UpdateScaleDisplay()
    {
        GameManager gameManager = GameManager.Instance;

        Quaternion rotation = Quaternion.Euler(0, 0, -gameManager.currentWinStateClamped * _tiltInterval);
        Quaternion reverseRotation = Quaternion.Euler(0, 0, gameManager.currentWinStateClamped * _tiltInterval);

        _scaleRod.localRotation = rotation;
        _scaleLeft.localRotation = reverseRotation;
        _scaleRight.localRotation = reverseRotation;

        _scaleLeftValue.sprite = _amountCount[System.Math.Min(gameManager.OpponentHits, _amountCount.Length - 1)];
        _scaleLeftValue.SetNativeSize();
        _scaleRightValue.sprite = _amountCount[System.Math.Min(gameManager.PlayerHits, _amountCount.Length - 1)];
        _scaleRightValue.SetNativeSize();
    }
}
