using TMPro;
using UnityEngine;

public class ChangeLanguageScript : MonoBehaviour
{
    public static ChangeLanguageScript instance;
    public TMP_Text totalBalanceText;
    //public TMP_Text pointsText;
    public TMP_Text withdrawText;
    //public TMP_Text maxText;
    //public TMP_Text minText;
    public TMP_Text totalBetText;
    public TMP_Text paytableText;
    public TMP_Text forNowText;
    public TMP_Text jackpotModeText;
    public TMP_Text wildText;
    public TMP_Text gotItText;
    public TMP_Text collectText;
    public TMP_Text rewardText;
    public TMP_Text betAmountText;
    public TMP_Text previewText;
    public TMP_Text hidePreviewText;
    public TMP_Text spinsLeftText;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateUITextChange()
    {

        // skillGameText.text = LanguageManager.instance.GetLocalizedString("skill_game");
        totalBalanceText.text = LanguageManager.instance.GetLocalizedString("total_balance");
        //pointsText.text = LanguageManager.instance.GetLocalizedString("points");
        withdrawText.text = LanguageManager.instance.GetLocalizedString("withdraw");
        //maxText.text = LanguageManager.instance.GetLocalizedString("max");
        //minText.text = LanguageManager.instance.GetLocalizedString("min");
        totalBetText.text = LanguageManager.instance.GetLocalizedString("total_bet");
        paytableText.text = LanguageManager.instance.GetLocalizedString("paytable");
        forNowText.text = LanguageManager.instance.GetLocalizedString("for_now");
        jackpotModeText.text = LanguageManager.instance.GetLocalizedString("jackpot_mode");
        wildText.text = LanguageManager.instance.GetLocalizedString("wild");
        gotItText.text = LanguageManager.instance.GetLocalizedString("got_it");
        collectText.text = LanguageManager.instance.GetLocalizedString("collect");
        rewardText.text = LanguageManager.instance.GetLocalizedString("reward");
        betAmountText.text = LanguageManager.instance.GetLocalizedString("bet_amount");
    }
}
