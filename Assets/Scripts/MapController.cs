using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class MapController : NetworkSceneSingleton<MapController>
{
    [SerializeField] int winScore;
    [SerializeField] int showWinnerSeconds;
    [SerializeField] GameObject scoreLabel;
    [SerializeField] TextMeshProUGUI scoreCounter;
    [SerializeField] GameObject winnerLabel;
    [SerializeField] TextMeshProUGUI winnerName;

    void OnDisable()
    {
        scoreLabel.SetActive(false);
        winnerLabel.SetActive(false);
    }

    void OnEnable()
    {
        scoreLabel.SetActive(true);
    }

    [Server]
    public bool CheckWin(DasherPlayer player)
    {
        var isWin = player.CurrentScore == winScore;

        if (isWin)
        {
            ShowWinLabel("Player" + player.netId);
            StartCoroutine(RoutineReloadMap());
        }

        return isWin;
    }

    [Client]
    public void UpdateScoreCounter(DasherPlayer player)
    {
        scoreCounter.text = player.CurrentScore.ToString();
    }

    [ClientRpc]
    private void ShowWinLabel(string name)
    {
        winnerName.text = name;
        winnerLabel.SetActive(true);
    }

    private IEnumerator RoutineReloadMap()
    {
        yield return new WaitForSeconds(showWinnerSeconds);

        NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }
}
