using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeckDefinition", menuName = "Scriptable Objects/CardDeckDefinition")]
public class CardDeckDefinition : ScriptableObject
{
    [SerializeField] public Dictionary<CardDefinition, int> CardDeck;
}
