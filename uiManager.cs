using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI bottomText;

    [SerializeField]
    private TextMeshProUGUI BlackScore;

    [SerializeField]
    private TextMeshProUGUI WhiteScore;

    [SerializeField]
    private TextMeshProUGUI Winner;

    [SerializeField]
    private Image BlackOverlay;

    [SerializeField]
    private RectTransform PlayAgainButton;

    public void SetPlayerText(Player currentPlayer)
    {
        if (currentPlayer == Player.Black)
        {
            bottomText.text = "Black's Turn <sprite name=DiscBlackUp>";
        }
        else if (currentPlayer == Player.White)
        {
            bottomText.text = "White's Turn <sprite name=DiscWhiteUp>";
        }
    }

    public void SetSkippedText(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            bottomText.text = "Black Cannot Move! <sprite name=DiscBlackUp>";
        }
        else if (skippedPlayer == Player.White)
        {
            bottomText.text = "White Cannot Move! <sprite name=DiscWhiteUp>";
        }
    }

    public void SetBottomText(string message)
    {
        bottomText.text = message;
    }

    public IEnumerator AnimateBottomText()
    {
        bottomText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(bottomText.rectTransform);
        yield return ScaleUp(BlackScore.rectTransform);
        yield return ScaleUp(WhiteScore.rectTransform);
    }

    public void SetBlackScoreText(int score)
    {
        BlackScore.text = $"<sprite name=DiscBlackUp> {score}";
    }

    public void SetWhiteScoreText(int score)
    {
        WhiteScore.text = $"<sprite name=DiscWhiteUp> {score}";
    }

    private IEnumerator ShowOverlay()
    {
        BlackOverlay.gameObject.SetActive(true);
        BlackOverlay.color = new Color(0, 0, 0, 0);
        BlackOverlay.rectTransform.LeanAlpha(0.8f, 1);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay()
    {
        BlackOverlay.CrossFadeAlpha(0, 1, false);
        yield return new WaitForSeconds(1);
        BlackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoresDown()
    {
        BlackScore.rectTransform.LeanMoveY(0, 0.5f);
        WhiteScore.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(Player winner)
    {
        switch (winner)
        {
            case Player.Black:
                Winner.text = "Black Won!";
                break;
            case Player.White:
                Winner.text = "White Won!";
                break;
            case Player.None:
                Winner.text = "It`s a Tie!";
                break;
        }
    }

    public IEnumerator ShowEndScreen()
    {
        yield return ShowOverlay();
        yield return MoveScoresDown();
        yield return ScaleUp(Winner.rectTransform);
        yield return ScaleUp(PlayAgainButton);
    }

    public IEnumerator HideEndScreen()
    {
        StartCoroutine(ScaleDown(Winner.rectTransform));
        StartCoroutine(ScaleDown(BlackScore.rectTransform));
        StartCoroutine(ScaleDown(WhiteScore.rectTransform));
        StartCoroutine(ScaleDown(PlayAgainButton));
        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }
}
