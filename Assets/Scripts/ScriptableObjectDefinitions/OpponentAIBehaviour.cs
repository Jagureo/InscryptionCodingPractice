using System;
using UnityEngine;

[CreateAssetMenu(fileName = "OpponentAIBehaviour", menuName = "Scriptable Objects/OpponentAIBehaviour")]
public class OpponentAIBehaviour : ScriptableObject
{
    [Min(1)] public int BoardSize;
    public CardWave[] Waves;

    private void OnValidate()
    {
        BoardSize = Mathf.Max(1, BoardSize);

        if (Waves == null)
            return;

        foreach (CardWave wave in Waves)
        {
            if (wave == null) { continue; }

            if (wave.Cards == null)
            {
                wave.Cards = new CardDefinition[BoardSize];
            }
            else if (wave.Cards.Length != BoardSize)
            {
                Array.Resize(ref wave.Cards, BoardSize);
            }
        }
    }
}

[Serializable]
public class CardWave
{
    public CardDefinition[] Cards;
}
