using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
    public class TeacherConfig : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(m => new
            {
                m.LastName,
                m.FirstName,
                m.MiddleName
            });
            builder.HasOne(m => m.UserInfo)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
