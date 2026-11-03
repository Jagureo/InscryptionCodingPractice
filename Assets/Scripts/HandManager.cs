using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;

    private List<Card> _cardHand;

    private void Awake()
    {
        CleanHand();
    }

    private void CleanHand()
    {
        _cardHand = new List<Card>();
        
        for (int i = transform.childCount - 1; i >= 0; --i) 
        { 
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void AddCard(CardDefinition cardDefinition)
    {
        if (_cardHand == null) { _cardHand = new List<Card>(); }

        Card card = Instantiate(_cardPrefab, transform).GetComponent<Card>();
        card.GetComponent<Card>().SetupCard(cardDefinition, true);
        _cardHand.Add(card);
    }

    public void HighlightCard(Card card)
    {
        for (int i = 0; i < _cardHand.Count; ++i) 
        { 
            _cardHand[i].SetTint(Color.black, card != null ? 0.5f : 0f);
        }

        if (card != null) { card.SetTint(Color.black, 0f); }
    }

    public void RemoveCard(Card card)
    {
        for (int i = 0; i < _cardHand.Count; ++i) 
        { 
            if (_cardHand[i] == card)
            {
                Destroy(transform.GetChild(i).gameObject);
                _cardHand.RemoveAt(i);
            }
        }
    }
}
