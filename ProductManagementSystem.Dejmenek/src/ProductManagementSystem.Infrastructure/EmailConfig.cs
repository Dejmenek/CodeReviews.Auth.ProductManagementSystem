using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Infrastructure;
public class EmailConfig
{
    [Required]
    [EmailAddress]
    public string From { get; init; } = null!;
    [Required]
    public string SmtpServer { get; init; } = null!;
    [Required]
    [Range(1, 65535)]
    public int Port { get; init; }
    [Required]
    public string Username { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;
}
