using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heroic.UI
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "Game";

        public void StartGame()
        {
            if (string.IsNullOrEmpty(gameSceneName))
            {
                Debug.LogError("MainMenuPresenter cannot start game because gameSceneName is empty.");
                return;
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene(gameSceneName);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
