namespace BackendApi.Core.Models
{
    public class GradePointEquivalent
    {
        public int Id { get; set; }
        public double? MinPercentage { get; set; }

        /// <summary>
        /// The maximum percentage for this grade point.
        /// </summary>
        public double MaxPercentage { get; set; }

        /// <summary>
        /// The corresponding grade point value.
        /// </summary>
        public double GradePoint { get; set; }
    }
}
