using System.ComponentModel.DataAnnotations;

public class SessionUtilisateur
{
    public int Id { get; set; }

    [Required]
    public string NomUtilisateur { get; set; } = string.Empty;

    public DateTime DateConnexion { get; set; }
    public string MotDePasse { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}
