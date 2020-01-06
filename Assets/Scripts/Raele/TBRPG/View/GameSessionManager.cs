using Raele.Util;

namespace Raele.TBRPG.View
{
    /// <summary>
    /// This is the entry point of the game and holds the main object of the game, "GameSession".
    /// </summary>
    public class GameSessionManager : SingletonPersistentMonoBehaviour<GameSessionManager>
    {
        public GameSession GameSession { get; private set; } = new GameSession();
    }
}
