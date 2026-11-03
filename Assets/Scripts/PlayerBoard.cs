using System.Collections;
using UnityEngine;

public class PlayerBoard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Card[] _cardActive;
    [SerializeField] private SacrificeSlot[] _sacrificeSlots;

    [Header("Enemy Board")]
    [SerializeField] private EnemyBoard _enemyBoard;

    GameManager gameManager = GameManager.Instance;

    private void Awake()
    {
        if (gameManager == null) { gameManager = GameManager.Instance; }
        ResetBoard();
    }

    private void ResetBoard()
    {
        for (int i = 0; i < _cardActive.Length; ++i)
        {
            _cardActive[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _sacrificeSlots.Length; ++i)
        {
            _sacrificeSlots[i].gameObject.SetActive(false);
        }
    }

    public IEnumerator Attack()
    {
        for (int i = 0; i < _cardActive.Length; ++i)
        {
            if (!_cardActive[i].gameObject.activeInHierarchy) { continue; }

            if ((_cardActive[i].CardDefinition.InitialSigils & CardDefinition.Sigils.TouchOfDeath) != 0)
            {
                _enemyBoard.Attacked(i, _cardActive[i].Attack, true);
            }
            else if ((_cardActive[i].CardDefinition.InitialSigils & CardDefinition.Sigils.Airborne) == 0)
            {
                _enemyBoard.Attacked(i, _cardActive[i].Attack, false);
            }
            else
            {
                // TODO: Check for Mighty Leap
                GameManager.Instance.PlayerHits += _cardActive[i].Attack;
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
            if (excessDamage > 0) { gameManager.OpponentHits += excessDamage; }
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
                    if (excessDamage > 0) { gameManager.OpponentHits += excessDamage; }
                    
                    if (_cardActive[index].Health <= 0 || touchOfDeath)
                    {
                        _cardActive[index].gameObject.SetActive(false);
                    }

                    return;
                }
            }

            gameManager.OpponentHits += damage;
        }
    }

    public void SetupCard(int index, CardDefinition cardDefinition)
    {
        _cardActive[index].SetupCard(cardDefinition);
        _cardActive[index].gameObject.SetActive(true);

        if ((cardDefinition.InitialSigils & CardDefinition.Sigils.RabbitHole) != 0)
        {
            gameManager.GetRabbit();
        }
    }

    public int GetActiveCreatureCount()
    {
        int count = 0;
        for (int i = 0; i < _cardActive.Length; ++i)
        {
            if (_cardActive[i].gameObject.activeInHierarchy) { ++count; }
        }

        return count;
    }

    public void SacrificeCreature(int index)
    {
        if (!_cardActive[index].gameObject.activeInHierarchy) { return; }

        _cardActive[index].gameObject.SetActive(false);
        _sacrificeSlots[index].gameObject.SetActive(false);
        gameManager.Blood += ((_cardActive[index].CardDefinition.InitialSigils & CardDefinition.Sigils.WorthySacrifice) == 0) ? 1 : 3;

        if (gameManager.Blood >= gameManager.HeldCard.CardDefinition.CostAmount)
        {
            for (int i = 0; i < _cardActive.Length; ++i)
            {
                _sacrificeSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void EnableSacrificeMode()
    {
        for (int i = 0; i < _sacrificeSlots.Length; ++i)
        {
            _sacrificeSlots[i].gameObject.SetActive(_cardActive[i].gameObject.activeInHierarchy);
        }
    }

    public void DisableSacrificeMode()
    {
        for (int i = 0; i < _sacrificeSlots.Length; ++i)
        {
            _sacrificeSlots[i].gameObject.SetActive(false);
        }
    }
}
