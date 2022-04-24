using System.ComponentModel.DataAnnotations;

namespace UserNotepad.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(150)]
        public string Surname { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    }
}
