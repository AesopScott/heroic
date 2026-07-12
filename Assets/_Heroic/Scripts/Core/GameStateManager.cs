using UnityEngine;

namespace Heroic.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public enum GameState
        {
            Playing,
            Paused,
            LevelUpDraft,
            Results
        }

        [SerializeField] private GameState state = GameState.Playing;

        public GameState CurrentState => state;

        public void SetState(GameState newState)
        {
            state = newState;
        }
    }
}
