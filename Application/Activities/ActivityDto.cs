using Application.Profiles;

namespace Application.Activities
{
    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? City { get; set; }
        public string? Venue { get; set; }
        public string? HostUsename { get; set; }
        public bool IsCancelled { get; set; }
        //Indicates if this user is being followed by the current logged user 
        public bool Following { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public ICollection<AttendeeDto>? Attendees { get; set; }
    }
}