using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionBox : MonoBehaviour
{
    [Header("Card Display UI")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _costIcon;
    [SerializeField] private TextMeshProUGUI _attackValueText;
    [SerializeField] private TextMeshProUGUI _healthValueText;

    [Header("References")]
    [SerializeField] private CardSharedAssets _cardSharedAssets;
    
    private void Awake()
    {
        SetDescription(null);
    }

    public void SetDescription(CardDefinition cardDefinition)
    {
        if (cardDefinition == null)
        {
            _titleText.text = null;
            _descriptionText.text = null;
            _costIcon.sprite = null;
            _costIcon.enabled = false;
            _attackValueText.text = null;
            _healthValueText.text = null;
        }
        else
        {
            _titleText.text = cardDefinition.Name;
            _descriptionText.text = cardDefinition.Description;
            _costIcon.sprite = _cardSharedAssets.GetCostIcon(cardDefinition.Cost, cardDefinition.CostAmount);
            _costIcon.enabled = _costIcon.sprite != null;
            _attackValueText.text = cardDefinition.AttackValue.ToString();
            _healthValueText.text = cardDefinition.HealthValue.ToString();
        }
    }
}
