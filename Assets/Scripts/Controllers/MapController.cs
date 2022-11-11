using Mirror;
using Player;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Controllers
{
    public class MapController : NetworkSceneSingleton<MapController>
    {
        [SerializeField] int winScore;
        [SerializeField] int showWinnerSeconds;
        [SerializeField] GameplayUI gameplayUI;

        void OnEnable()
        {
            gameplayUI.ShowUI();
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
            gameplayUI.UpdateScoreCounter(player.CurrentScore);
        }

        [ClientRpc]
        private void ShowWinLabel(string name)
        {
            gameplayUI.ShowWinLabel(name);
        }

        private IEnumerator RoutineReloadMap()
        {
            yield return new WaitForSeconds(showWinnerSeconds);

            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }
    }
}
