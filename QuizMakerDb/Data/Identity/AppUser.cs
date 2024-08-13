using Microsoft.AspNetCore.Identity;

namespace QuizMakerDb.Data.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public byte[]? ProfilePicture { get; set; }
    }
}
