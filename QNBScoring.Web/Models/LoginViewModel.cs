using System.ComponentModel.DataAnnotations;

namespace QNBScoring.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        public string NomUtilisateur { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }
    }
}
