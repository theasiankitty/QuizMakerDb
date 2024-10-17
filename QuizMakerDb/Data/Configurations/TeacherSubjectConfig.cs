using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
	public class TeacherSubjectConfig : IEntityTypeConfiguration<TeacherSubject>
	{
		public void Configure(EntityTypeBuilder<TeacherSubject> builder)
		{
			builder.HasKey(e => e.Id);

			builder.HasOne(m => m.TeacherInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(m => m.CourseYearSubjectInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
