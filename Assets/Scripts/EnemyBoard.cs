using System.Collections;
using UnityEngine;

public class EnemyBoard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardQueue[] _cardQueue;
    [SerializeField] private Card[] _cardActive;

    [Header("Player Board")]
    [SerializeField] private PlayerBoard _playerBoard;

    private GameManager gameManager = GameManager.Instance;

    private void Awake()
    {
        if (gameManager == null) { gameManager = GameManager.Instance; }
        ResetBoard();
    }

    private void ResetBoard()
    {
        for (int i = 0; i < _cardQueue.Length; ++i)
        {
            _cardQueue[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _cardActive.Length; ++i)
        {
            _cardActive[i].gameObject.SetActive(false);
        }
    }

    private CardQueue GetRandomInactiveQueueSlot()
    {
        int startIndex = Random.Range(0, _cardQueue.Length);

        for (int i = 0; i < _cardQueue.Length; i++)
        {
            int index = (startIndex + i) % _cardQueue.Length;

            if (!_cardQueue[index].gameObject.activeInHierarchy)
            {
                return _cardQueue[index];
            }
        }

        return null;
    }

    private Card GetRandomInactiveActiveSlot()
    {
        int startIndex = Random.Range(0, _cardActive.Length);

        for (int i = 0; i < _cardActive.Length; i++)
        {
            int index = (startIndex + i) % _cardActive.Length;

            if (!_cardActive[index].gameObject.activeInHierarchy) 
            { 
                return _cardActive[index]; 
            }
        }

        return null;
    }

    public void QueueCreature(CardDefinition cardDefinition)
    {
        CardQueue queueSlot = GetRandomInactiveQueueSlot();
        if (queueSlot == null) { return; }

        queueSlot.SetupCard(cardDefinition);
        queueSlot.gameObject.SetActive(true);
    }

    public void SetActiveCreatre(CardDefinition cardDefinition)
    {
        Card activeSlot = GetRandomInactiveActiveSlot();
        if (activeSlot == null) { return; }

        activeSlot.SetupCard(cardDefinition);
        activeSlot.gameObject.SetActive(true);
    }

    public void MoveCreatureForward()
    {
        for (int i = 0; i < _cardQueue.Length; ++i)
        {
            if (!_cardQueue[i].gameObject.activeInHierarchy) { continue; }

            if (!_cardActive[i].gameObject.activeInHierarchy)
            {
                _cardActive[i].gameObject.SetActive(true);
                _cardActive[i].SetupCard(_cardQueue[i].CardDefinition);
                _cardActive[i].Health = _cardQueue[i].Health;

                _cardQueue[i].gameObject.SetActive(false);
            }
        }
    }

    public void Evolve()
    {
        for (int i = 0; i < _cardActive.Length; ++i)
        {
            if (!_cardActive[i].gameObject.activeInHierarchy) { continue; }
            
            _cardActive[i].Evolve();
        }
    }

    public IEnumerator Attack()
    {
        for (int i = 0; i < _cardActive.Length; ++i)
        {
            if (!_cardActive[i].gameObject.activeInHierarchy) { continue; }

            if ((_cardActive[i].CardDefinition.InitialSigils & CardDefinition.Sigils.Airborne) == 0)
            {
                _playerBoard.Attacked(i, _cardActive[i].Attack);
            }
            else
            {
                // TODO: Check for Mighty Leap
                GameManager.Instance.OpponentHits += _cardActive[i].Attack;
            }

            yield return new WaitForSeconds(GameManager.GAME_DELAY);
        }
    }

    public void Attacked(int index, int damage, bool touchOfDeath = false)
    {
        if (damage == 0) { return; }
        if (_cardActive[index].gameObject.activeInHierarchy)
        {
            int excessDamage = _cardActive[index].TakeDamage(damage);
            if (excessDamage > 0) 
            {
                if (_cardQueue[index].gameObject.activeInHierarchy)
                {
                    excessDamage = _cardQueue[index].TakeDamage(damage);
                    if (_cardQueue[index].Health <= 0)
                    {
                        _cardQueue[index].gameObject.SetActive(false);
                    }
                }
                gameManager.PlayerHits += excessDamage; 
            }
            
            if (_cardActive[index].Health <= 0 || touchOfDeath)
            {
                _cardActive[index].gameObject.SetActive(false);
            }
        }
        else
        {
            // Check for Burrower to tank damage
            for (int i = 0; i < _cardActive.Length; ++i)
            {
                if (!_cardActive[i].gameObject.activeInHierarchy) { continue; }
                
                if ((_cardActive[i].CardDefinition.InitialSigils & CardDefinition.Sigils.Burrower) != 0)
                {
                    _cardActive[index].gameObject.SetActive(true);
                    _cardActive[index].SetupCard(_cardActive[i].CardDefinition);
                    _cardActive[index].Health = _cardActive[i].Health;
                    _cardActive[i].gameObject.SetActive(false);

                    int excessDamage = _cardActive[index].TakeDamage(damage);
                    if (excessDamage > 0) 
                    {
                        if (_cardQueue[index].gameObject.activeInHierarchy)
                        {
                            excessDamage = _cardQueue[index].TakeDamage(damage);
                            if (_cardQueue[index].Health <= 0)
                            {
                                _cardQueue[index].gameObject.SetActive(false);
                            }
                        }
                        gameManager.PlayerHits += excessDamage; 
                    }
                    
                    if (_cardActive[index].Health <= 0 || touchOfDeath)
                    {
                        _cardActive[index].gameObject.SetActive(false);
                    }

                    return;
                }
            }

            gameManager.PlayerHits += damage;
        }
    }
}
