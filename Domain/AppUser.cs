using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string? Bio { get; set; }

        public ICollection<ActivityAttendee> Activities { get; set; }
        public ICollection<Photo> Photos { get; set; }

        //Users followed by the logged user
        public ICollection<UserFollowing> Followings { get; set; }

        //Users that follow the logged user 
        public ICollection<UserFollowing> Followers { get; set; }

    }
}