using UnityEngine;

[CreateAssetMenu(fileName = "CardSharedAssets", menuName = "Scriptable Objects/CardSharedAssets")]
public class CardSharedAssets : ScriptableObject
{
    [SerializeField] private Sprite[] _sigilIcons;
    [SerializeField] private Sprite[] _bloodCostIcons;

    public Sprite GetSigil(int index) => index < _sigilIcons.Length ? _sigilIcons[index] : null;
    public Sprite GetCostIcon(CardDefinition.CostType type, ushort amount)
    {
        switch (type)
        {
            case CardDefinition.CostType.BLOOD:
                return GetBloodCostIcon(amount);

            case CardDefinition.CostType.BONES:
                return null;

            case CardDefinition.CostType.MOX:
                return null;

            case CardDefinition.CostType.ENERGY:
                return null;

            default:
                return null;
        }
    }
    public Sprite GetBloodCostIcon(int amount) => amount < _bloodCostIcons.Length ? _bloodCostIcons[amount] : null;
}
