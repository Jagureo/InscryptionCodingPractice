using UnityEngine;
using UnityEngine.EventSystems;

public class SacrificeSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerBoard playerBoard;

    public int index = -1;

    public void OnPointerClick(PointerEventData eventData)
    {
        playerBoard.SacrificeCreature(index);
    }
}
