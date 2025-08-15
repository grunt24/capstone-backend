using BackendApi.Core.Models;

namespace BackendApi.IRepositories
{
    public interface IGradePointService
    {
        List<GradePointEquivalent> GetGradingScale();
        double? GetGradePoint(double percentage);
    }
}
