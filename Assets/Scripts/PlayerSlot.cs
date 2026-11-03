using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerBoard playerBoard;

    public int index = -1;
    private GameManager gameManager = GameManager.Instance;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager == null) { gameManager = GameManager.Instance; }
        if (gameManager.HeldCard != null)
        {
            if (gameManager.HeldCard.CardDefinition.Cost == CardDefinition.CostType.BLOOD
             && gameManager.HeldCard.CardDefinition.CostAmount <= gameManager.Blood)
            {
                playerBoard.SetupCard(index, gameManager.HeldCard.CardDefinition);
                gameManager.RemoveHeldCardFromHand();
            }
        }
    }
}
