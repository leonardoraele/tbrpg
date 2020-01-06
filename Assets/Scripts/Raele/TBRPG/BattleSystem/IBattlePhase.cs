
namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBattlePhase
    {
        bool HasStarted { get; }
        bool HasFinished { get; }
        void Start();
        void Update();
        void Finish();
    }
}
