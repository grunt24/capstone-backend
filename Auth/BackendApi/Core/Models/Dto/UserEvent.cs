using System.Text.Json.Serialization;

namespace BackendApi.Core.Models.Dto
{
    public class UserEvent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? EventDescription { get; set; }
        public string Timestamp { get; set; }

        // Navigation property
        [JsonIgnore]
        public StudentModel? User { get; set; }
    }

}
