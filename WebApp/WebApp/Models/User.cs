using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class User
    {
        [Key]
        public string IdUser { get; set; }

        [Required(ErrorMessage = "Debe ingresar su nombre.")]
        [StringLength(250)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Debe ingresar sus apellidos.")]
        [StringLength(250)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Debe ingresar un email válido.")]
        [StringLength(250)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar su cédula.")]
        [StringLength(50)]
        public string Document { get; set; }
    }
}


