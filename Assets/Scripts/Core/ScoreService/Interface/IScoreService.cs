using System.Threading.Tasks;

namespace Core.ScoreService.Service
{
    public interface IScoreService
    {
        Task Inject();
        int CurrentScore { get; }
        int CurrentCombo { get; }
    }
}