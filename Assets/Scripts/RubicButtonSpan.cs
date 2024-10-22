using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RubicButtonSpan : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int _movesToDisable = 3;
    Button _button;

    private void OnEnable()
    {
        CheckForWinningPatterns.CoolDownRubicButton += OnCoolDownOver;
        RubikCubeController.StartCoolDown += DisableButton;
        _button = GetComponent<Button>();
        _button.interactable = true;
        _movesToDisable = 3;
    }
    private void OnDisable()
    {
        RubikCubeController.StartCoolDown -= DisableButton;
        CheckForWinningPatterns.CoolDownRubicButton -= OnCoolDownOver;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button.interactable)
            _movesToDisable--;
    }

    private void DisableButton()
    {
        _button.interactable = false;
    }

    private void OnCoolDownOver()
    {
        if (_movesToDisable > 0)
        {
            _button.interactable = true;
        }
    }
}
