using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private HandManager _handManager;
    [SerializeField] private HealthManager _healthManager;
    [SerializeField] private DescriptionBox _descriptionBox;
    [SerializeField] private BellButton _bellButton;
    [SerializeField] private EnemyBoard _enemyBoard;
    [SerializeField] private PlayerBoard _playerBoard;

    [Header("Game Setup")]
    [SerializeField] private CardDeckDefinition _cardDeck;
    private List<CardDefinition> _runtimeDeck;
    [SerializeField] private OpponentAIBehaviour _opponentAIBehaviour;

    [Header("Fodder")]
    [SerializeField] private CardDefinition _rabbitCard;

    [Header("Constant Values")]
    public const int MAX_HEALTH = 5;    

    public const float GAME_SPEED = 1.0f;
    public const float GAME_DELAY = 0.5f;

    public const int STARTING_HAND_SIZE = 4;

    [Header("Variables")]
    /// <summary>
    /// Number of times player got hit
    /// </summary>
    private int _playerHits;
    public int PlayerHits
    {
        get => _playerHits;
        set
        {
            _playerHits = value;
            _healthManager.UpdateHealthDisplay();

            if (currentWinState >= MAX_HEALTH || currentWinState <= -MAX_HEALTH)
            {
                if (currentWinState >= MAX_HEALTH) { Debug.Log("GAME OVER - YOU WIN"); }
                else { Debug.Log("GAME OVER - YOU LOSE"); }

                CurrentGameState = GameState.END;
                _bellButton.DisableButton();
            }
        }
    }

    /// <summary>
    /// Number of times opponent got hit
    /// </summary>
    private int _opponentHits;
    public int OpponentHits
    {
        get => _opponentHits;
        set
        {
            _opponentHits = value;
            _healthManager.UpdateHealthDisplay();

            if (currentWinState >= MAX_HEALTH || currentWinState <= -MAX_HEALTH)
            {
                if (currentWinState >= MAX_HEALTH) { Debug.Log("GAME OVER - YOU WIN"); }
                else { Debug.Log("GAME OVER - YOU LOSE"); }

                CurrentGameState = GameState.END;
                _bellButton.DisableButton();
            }
        }
    }

    public int currentWinState => PlayerHits - OpponentHits;
    public int currentWinStateClamped => System.Math.Clamp(currentWinState, -MAX_HEALTH, MAX_HEALTH);

    public Card HeldCard = null;

    public int TurnNumber = 0;

    public int Blood = 0;

    public enum GameState
    {
        BUSY = 0,
        READY = 1,
        SACRIFICE = 2,
        END = 3
    }

    public GameState CurrentGameState = GameState.BUSY;

    private void Awake()
    {
        // Prevent duplicate instances
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Setup Game
        ResetDeck();
    }

    private void Start()
    {
        for (int i = 0; i < STARTING_HAND_SIZE; ++i)
        {
            AddCardToHand();
        }

        SummonWave();

        CurrentGameState = GameState.READY;
    }

    private void ResetDeck()
    {
        _runtimeDeck = new List<CardDefinition>();

        foreach (KeyValuePair<CardDefinition, int> entry in _cardDeck.CardDeck)
        {
            for (int i = 0; i < entry.Value; ++i)
            {
                _runtimeDeck.Add(entry.Key);
            }
        }
    }

    private void AddCardToHand()
    {
        if (_runtimeDeck.Count < 1) { return; }
        int randomIndex = UnityEngine.Random.Range(0, _runtimeDeck.Count);
        _handManager.AddCard(_runtimeDeck[randomIndex]);
        _runtimeDeck.RemoveAt(randomIndex);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void CardHover(Card card)
    {
        if (card == null)  { return; }
        if (CurrentGameState == GameState.BUSY) { return; }

        SetDescription(card.CardDefinition);
    }

    public void CardHover(CardQueue card)
    {
        if (card == null)  { return; }
        if (CurrentGameState == GameState.BUSY) { return; }

        SetDescription(card.CardDefinition);
    }

    public void CardSelected(Card card)
    {
        if (card == null) { return; }
        if (Blood > 0) { return; }

        if (card.CardInHand && card.CardDefinition.CostAmount > 0)
        {
            if (!CheckCardPlayable(card.CardDefinition.Cost, card.CardDefinition.CostAmount)) 
            {
                Debug.Log("Not enough creatures to sacrifice!");
                return;
            }

            _playerBoard.EnableSacrificeMode();
            CurrentGameState = GameState.SACRIFICE;
        }

        if (HeldCard == null)
        {
            HeldCard = card.CardInHand ? card : null;
        }
        else
        {
            if (HeldCard == card)
            {
                HeldCard = null;
                _playerBoard.DisableSacrificeMode();
                CurrentGameState = GameState.READY;
            }
            else if (CurrentGameState == GameState.READY)
            {
                HeldCard = card.CardInHand ? card : null;
            }
        }
        
        _handManager.HighlightCard(HeldCard);
        _bellButton.SetButtonInteractable(HeldCard == null);
    }

    private void SetDescription(CardDefinition cardDefinition)
    {
        _descriptionBox.SetDescription(cardDefinition);
    }

    private void SummonWave()
    {
        if (TurnNumber >= _opponentAIBehaviour.Waves.Length) { return; }
        CardDefinition[] cardDefinitions = _opponentAIBehaviour.Waves[TurnNumber].Cards;
        for (int i = 0; i < cardDefinitions.Length; ++i)
        {
            if (cardDefinitions[i] == null) { continue; }
            _enemyBoard.QueueCreature(cardDefinitions[i]);
        }
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnCoroutine());
    }

    public IEnumerator EndTurnCoroutine()
    {
        _bellButton.DisableButton();
        CurrentGameState = GameState.BUSY;

        yield return StartCoroutine(_playerBoard.Attack());

        if (CurrentGameState == GameState.END)
            yield break;

        _enemyBoard.Evolve();
        _enemyBoard.MoveCreatureForward();

        yield return StartCoroutine(_enemyBoard.Attack());

        if (CurrentGameState == GameState.END)
            yield break;

        ++TurnNumber;
        SummonWave();
        AddCardToHand();

        CurrentGameState = GameState.READY;
        _bellButton.EnableButton();
    }

    public void RemoveHeldCardFromHand()
    {
        if (HeldCard == null) { return; }
        _handManager.RemoveCard(HeldCard);
        HeldCard = null;
        Blood = 0;

        _handManager.HighlightCard(null);
        _bellButton.SetButtonInteractable(true);
    }

    public bool CheckCardPlayable(CardDefinition.CostType type, int amount)
    {
        switch (type)
        {
            case CardDefinition.CostType.BLOOD:
                return _playerBoard.GetActiveCreatureCount() >= amount;
            case CardDefinition.CostType.BONES:
                return true;
            case CardDefinition.CostType.MOX:
                return true;
            case CardDefinition.CostType.ENERGY:
                return true;
            default:
                return true;
        }
    }

    public void GetRabbit()
    {
        _handManager.AddCard(_rabbitCard);
    }
}
