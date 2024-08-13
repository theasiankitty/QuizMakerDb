using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; } = string.Empty!;

        public bool Active { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
