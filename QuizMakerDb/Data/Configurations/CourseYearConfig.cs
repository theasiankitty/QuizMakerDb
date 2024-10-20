using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
	public class CourseYearConfig : IEntityTypeConfiguration<CourseYear>
	{
		public void Configure(EntityTypeBuilder<CourseYear> builder)
		{
			builder.HasKey(e => e.Id);
			builder.HasIndex(m => new
			{
				m.Name,
				m.Year
			});
			builder.HasOne(m => m.CourseInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
