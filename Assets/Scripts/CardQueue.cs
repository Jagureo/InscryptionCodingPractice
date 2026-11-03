using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardQueue : MonoBehaviour, IPointerEnterHandler
{
    [Header("Card Display UI")]
    [SerializeField] private Image _minionImage;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Image _tint;

    [Header("Card Details")]
    [SerializeField] private CardDefinition _cardDefinition;
    public CardDefinition CardDefinition => _cardDefinition;

    private GameManager gameManager = GameManager.Instance;

    public int Health = -1;

    private void Awake()
    {
        if (_cardDefinition != null) { SetupCard(); }
    }

    public void SetupCard(CardDefinition cardDefinition = null)
    {
        if (cardDefinition != null) { _cardDefinition = cardDefinition; }
        if (_cardDefinition == null) { Debug.Log("[Card] No Card Definition!"); return; }

        if (_minionImage != null) { _minionImage.sprite = _cardDefinition.Image; }

        if (_attackText != null) { _attackText.text = _cardDefinition.AttackValue.ToString(); }

        if (_healthText != null) 
        { 
            Health = _cardDefinition.HealthValue;
            _healthText.text = Health.ToString(); 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameManager == null) { gameManager = GameManager.Instance; }
        gameManager.CardHover(this);
    }

    public void SetTint(Color color, float alpha = 1.0f)
    {
        _tint.color = new Color(color.r, color.g, color.b, alpha);
    }

    public int TakeDamage(int amount)
    {
        int newHealth = Health - amount;
        Health = System.Math.Max(newHealth, 0);
        _healthText.text = Health.ToString();
        return -System.Math.Min(newHealth, 0);
    }
}
