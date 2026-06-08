using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_Pharmacy.Models
{
        [Table("user")] 
        public class User
        {
            [Key]
            [Column("user_id")]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int UserId { get; set; }

            [Required]
            [Column("first_name")]
            [StringLength(50)]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [Column("last_name")]
            [StringLength(50)]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [Column("email")]
            [StringLength(100)]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Column("mobile")]
            [StringLength(20)]
            public string Mobile { get; set; } = string.Empty;

            [Required]
            [Column("username")]
            [StringLength(50)]
            public string Username { get; set; } = string.Empty;

            [Required]
            [Column("password")]
            [StringLength(255)]
            public string Password { get; set; } = string.Empty;

            [Column("attempt")]
            public int Attempt { get; set; } = 0;
        }
}
