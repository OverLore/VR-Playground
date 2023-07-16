using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DangerBallGameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private DangerBallPlayerController player;
    [SerializeField] private BallController ball;
    [SerializeField] private AudioSource goalSource;
    [SerializeField] private AudioClip goalClip;

    [Space(10), Header("Settings")]
    [SerializeField] float timeBeforeRestart = 2f;

    private int playerScore;
    private int aiScore;

    private bool playerHasLastGoal;

    void Start()
    {
        countdownText.text = string.Empty;

        UpdateScoreText();

        StartCoroutine(HandleLoadingScreen());
    }

    public void TriggerGoal(bool isPlayerSide)
    {
        if (isPlayerSide) aiScore++;
        else playerScore++;

        playerHasLastGoal = !isPlayerSide;

        goalSource.PlayOneShot(goalClip);

        ball.Boom();
        StartCoroutine(HandleDelayedRestart());
        UpdateScoreText();
        StartBallLaunch(true);
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"<color=blue>{playerScore}<color=white>-<color=red>{aiScore}";
    }

    public void StartBallLaunch(bool delayed)
    {
        StartCoroutine(HandleBallLaunch(delayed));
    }

    private void LaunchBall()
    {
        ball.Launch(new Vector3(0, 0, playerHasLastGoal ? 1f : -1f));
    }

    IEnumerator HandleBallLaunch(bool delayed)
    {
        if (delayed)
            yield return new WaitForSeconds(timeBeforeRestart);

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        countdownText.text = string.Empty;

        LaunchBall();
    }

    IEnumerator HandleLoadingScreen()
    {
        loadingScreen.alpha = 1f;

        yield return new WaitForSeconds(5f);

        loadingScreen.alpha = 0f;

        player.ResetPosition();
        StartBallLaunch(false);
    }

    IEnumerator HandleDelayedRestart()
    {
        yield return new WaitForSeconds(timeBeforeRestart);

        ball.Restart();
    }
}
