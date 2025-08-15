using BackendApi.Core.Models;
using BackendApi.IRepositories;

namespace BackendApi.Services
{
    public class GradePointService : IGradePointService
    {
        private readonly List<GradePointEquivalent> _gradingScale;

        public GradePointService()
        {
            _gradingScale = new List<GradePointEquivalent>
    {
        new GradePointEquivalent { MinPercentage = 98, MaxPercentage = 100, GradePoint = 1.00 },
        new GradePointEquivalent { MinPercentage = 95, MaxPercentage = 97, GradePoint = 1.25 },
        new GradePointEquivalent { MinPercentage = 92, MaxPercentage = 94, GradePoint = 1.50 },
        new GradePointEquivalent { MinPercentage = 89, MaxPercentage = 91, GradePoint = 1.75 },
        new GradePointEquivalent { MinPercentage = 86, MaxPercentage = 88, GradePoint = 2.00 },
        new GradePointEquivalent { MinPercentage = 83, MaxPercentage = 85, GradePoint = 2.25 },
        new GradePointEquivalent { MinPercentage = 80, MaxPercentage = 82, GradePoint = 2.50 },
        new GradePointEquivalent { MinPercentage = 77, MaxPercentage = 79, GradePoint = 2.75 },
        new GradePointEquivalent { MinPercentage = 75, MaxPercentage = 76, GradePoint = 3.00 },
        new GradePointEquivalent { MinPercentage = 74, MaxPercentage = 74, GradePoint = 4.00 }
    };
        }

        public GradePointService(List<GradePointEquivalent> gradingScale)
        {
            _gradingScale = gradingScale ?? throw new ArgumentNullException(nameof(gradingScale));
        }

        public List<GradePointEquivalent> GetGradingScale()
        {
            return _gradingScale;
        }

        public double? GetGradePoint(double percentage)
        {
            var equivalent = _gradingScale.FirstOrDefault(g =>
                (g.MinPercentage == null || percentage >= g.MinPercentage) &&
                (percentage <= g.MaxPercentage)
            );

            return equivalent?.GradePoint;
        }
    }
}
