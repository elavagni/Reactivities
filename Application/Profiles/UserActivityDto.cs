using System.Text.Json.Serialization;

namespace Application.Profiles
{
    public class UserActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        //We don't want to return this property as part of the dto, but it help us 
        //when using Projections
        [JsonIgnore]
        public string HostUserName { get; set; }
    }
}