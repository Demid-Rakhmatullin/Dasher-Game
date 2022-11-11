using TMPro;
using UnityEngine;

namespace UI
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] GameObject scoreLabel;
        [SerializeField] TextMeshProUGUI scoreCounter;
        [SerializeField] GameObject winnerLabel;
        [SerializeField] TextMeshProUGUI winnerName;

        private void Start()
        {
            scoreLabel.SetActive(false);
            winnerLabel.SetActive(false);
        }

        public void ShowUI()
        {
            scoreLabel.SetActive(true);
        }

        public void ShowWinLabel(string name)
        {
            winnerName.text = name;
            winnerLabel.SetActive(true);
        }

        public void UpdateScoreCounter(int count)
        {
            scoreCounter.text = count.ToString();
        }
    }
}
