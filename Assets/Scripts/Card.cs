using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Card Display UI")]
    [SerializeField] private Image _minionImage;
    [SerializeField] private Image _costImage;
    [SerializeField] private Image[] _sigilsIcon;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Image _tint;

    [Header("Card Details")]
    [SerializeField] private CardDefinition _cardDefinition;
    public CardDefinition CardDefinition => _cardDefinition;

    [Header("References")]
    [SerializeField] private CardSharedAssets _cardSharedAssets;

    private GameManager gameManager = GameManager.Instance;

    private bool _cardInHand = false;
    public bool CardInHand => _cardInHand;

    public int Attack = -1;
    public int Health = -1;

    private void Awake()
    {
        if (gameManager == null) { gameManager = GameManager.Instance; }
        if (_cardSharedAssets == null) { Debug.LogError("[Card] Missing CardSharedAssets!"); }
        if (_cardDefinition != null) { SetupCard(); }
    }

    public void SetupCard(CardDefinition cardDefinition = null, bool cardInHand = false)
    {
        if (cardDefinition != null) { _cardDefinition = cardDefinition; }
        if (_cardDefinition == null) { Debug.Log("[Card] No Card Definition!"); return; }

        _cardInHand = cardInHand;

        if (_minionImage != null) { _minionImage.sprite = _cardDefinition.Image; }

        if (_costImage != null) 
        {
            _costImage.sprite = _cardSharedAssets.GetCostIcon(_cardDefinition.Cost, _cardDefinition.CostAmount);
            _costImage.gameObject.SetActive(_costImage.sprite != null);
        }

        if (_sigilsIcon != null)
        {
            for (int i = 0; i < _sigilsIcon.Length; ++i)
            {
                _sigilsIcon[i].gameObject.SetActive(false);
            }

            for (int i = 0, j = 0; i < _sigilsIcon.Length && j < Enum.GetValues(typeof(CardDefinition.Sigils)).Length; ++j)
            {
                bool sigilBit = (_cardDefinition.InitialSigils & (CardDefinition.Sigils)(1 << j)) != 0;
                if (sigilBit) 
                {
                    _sigilsIcon[i].gameObject.SetActive(true);
                    _sigilsIcon[i].sprite = _cardSharedAssets.GetSigil(j);
                    ++i;
                }
            }
        }

        if (_attackText != null) 
        { 
            Attack = _cardDefinition.AttackValue;
            _attackText.text = Attack.ToString(); 
        }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager == null) { gameManager = GameManager.Instance; }
        gameManager.CardSelected(this);
    }

    public void SetTint(Color color, float alpha = 1.0f)
    {
        _tint.color = new Color(color.r, color.g, color.b, alpha);
    }

    /// COMBAT MECHANICS ///
    public int TakeDamage(int amount)
    {
        int newHealth = Health - amount;
        Health = System.Math.Max(newHealth, 0);
        _healthText.text = Health.ToString();
        return -System.Math.Min(newHealth, 0);
    }
    
    public void Evolve()
    {
        if (_cardDefinition.EvolvedCard != null)
        {
            SetupCard(_cardDefinition.EvolvedCard);
        }
    }
}
