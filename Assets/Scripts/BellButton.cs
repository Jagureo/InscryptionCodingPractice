using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BellButton : MonoBehaviour
{
    private GameManager gameManager = GameManager.Instance;
    private Button _button;

    private void Awake()
    {
        if (gameManager == null) { gameManager = GameManager.Instance; }
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnBellButtonClicked);
    }

    public void SetButtonInteractable(bool state) => _button.interactable = state;
    public void EnableButton() => SetButtonInteractable(true);
    public void DisableButton() => SetButtonInteractable(false);

    private void OnBellButtonClicked()
    {
        gameManager.EndTurn();
    }
}
