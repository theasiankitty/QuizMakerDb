using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class StudentVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }

		[Required(ErrorMessage = "First Name is required.")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = string.Empty!;

		[Display(Name = "Middle Name")]
		public string? MiddleName { get; set; }

		[Required(ErrorMessage = "Last Name is required.")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; } = string.Empty!;

		public string? Student { get; set; }

		public byte Sex { get; set; }

		[Display(Name = "Sex")]
		public string? SexDescription { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		public string Email { get; set; } = string.Empty!;

		[Required(ErrorMessage = "Username is required.")]
		[Display(Name = "User Name")]
		public string UserName { get; set; } = string.Empty!;

		public Guid UserId { get; set; }

<<<<<<< HEAD
<<<<<<< Updated upstream
=======
        public bool isIrregular { get; set; }

		public int? CurrentSectionId { get; set; }

>>>>>>> Stashed changes
		public bool Active { get; set; }
=======
        public bool isIrregular { get; set; }

        public bool Active { get; set; }
>>>>>>> 64989993402aa9d467121889e5da6b3cb58f9ffd

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
