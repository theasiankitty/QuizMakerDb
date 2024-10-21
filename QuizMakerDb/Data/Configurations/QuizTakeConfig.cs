using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
    public class QuizTakeConfig : IEntityTypeConfiguration<QuizTake>
    {
        public void Configure(EntityTypeBuilder<QuizTake> builder)
        {
            //builder.HasKey(e => e.Id);

            //builder.HasOne(m => m.QuizInfo);
            //builder.HasOne(m => m.StudentInfo);

            //builder.HasIndex(m => m.QuizId)
            //    .IsUnique(false);

            //builder.HasIndex(m => m.StudentId)
            //   .IsUnique(false);

            builder.HasKey(e => e.Id);

            builder.HasOne(m => m.QuizInfo)
                .WithMany()
                .HasForeignKey(m => m.QuizId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.StudentInfo)
                .WithMany()
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(m => m.QuizId)
                .IsUnique(false);

            builder.HasIndex(m => m.StudentId)
                .IsUnique(false);

        }
    }
}
