using Microsoft.AspNetCore.Identity;

namespace QuizMakerDb.Data.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public bool Active { get; set; }

        public byte[]? ProfilePicture { get; set; }
    }
}
