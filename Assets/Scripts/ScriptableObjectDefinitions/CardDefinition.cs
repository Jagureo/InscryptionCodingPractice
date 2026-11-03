using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class CardDefinition : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Image;

    public enum CostType
    {
        BLOOD = 0,
        BONES = 1,
        MOX = 2,
        ENERGY = 3
    }

    public CostType Cost;

    [Range(0, 11)] public ushort CostAmount = 0;

    [System.Flags]
    public enum Sigils : ushort
    {
        None = 0,
        MightyLeap = 1 << 0,
        BifurcatedStrike = 1 << 1,
        TrifurcatedStrike = 1 << 2,
        SteelTrap = 1 << 3,
        RabbitHole = 1 << 4,
        Sprinter = 1 << 5,
        TouchOfDeath = 1 << 6,
        Fledgling = 1 << 7,
        Burrower = 1 << 8,
        Fecundity = 1 << 9,
        Unkillable = 1 << 10,
        SharpQuills = 1 << 11,
        Hefty = 1 << 12,
        Guardian = 1 << 13,
        Airborne = 1 << 14,
        WorthySacrifice = 1 << 15
    }

    public Sigils InitialSigils;

    [Range(0, 9)] public ushort AttackValue;
    [Range(0, 9)] public ushort HealthValue;

    public CardDefinition EvolvedCard;
}
