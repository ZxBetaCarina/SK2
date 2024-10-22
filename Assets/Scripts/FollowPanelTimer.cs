using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FollowPanelTimer : MonoBehaviour
{
    private float _timer;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _followButon;
    public static Action TimerEnded;
    private bool TimerEndInvoked = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        _timer = 35 - ((1 + GameController.Instance._current_bet_index) * 5);
        _followButon = GetComponent<Button>();
        TimerEndInvoked = false;
    }

    // Update is called once per frame
    void Update()
    {
        print(GameController.Instance._currentPoints);
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            _timerText.text = ((int)_timer).ToString() + "s";
        }
        else
        {
            if (TimerEndInvoked) return;
            TimerEnded?.Invoke();
            _timerText.text = "";
            _followButon.interactable = true;
            TimerEndInvoked = true;
            gameObject.SetActive(false);
            PlayerPrefs.SetFloat("Balance", GameController.Instance._currentPoints);
        }
    }
}
