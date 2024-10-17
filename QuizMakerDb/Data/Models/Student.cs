using QuizMakerDb.Data.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string FirstName { get; set; } = string.Empty!;

        public string? MiddleName { get; set; }

        [StringLength(30)]
        public string LastName { get; set; } = string.Empty!;

        public byte Sex { get; set; }

        [StringLength(100)]
        public string Email { get; set; } = string.Empty!;

        [StringLength(20)]
        public string UserName { get; set; } = string.Empty!;

        public Guid UserId { get; set; }

        public bool isIrregular { get; set; }

        public int? CurrentCourseYearId { get; set; }

        public bool Active { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(UserId))]
		public AppUser? UserInfo { get; set; }
	}
}
