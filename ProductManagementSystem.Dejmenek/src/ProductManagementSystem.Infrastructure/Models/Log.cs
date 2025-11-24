using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementSystem.Infrastructure.Models;
public class Log
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public string? MessageTemplate { get; set; }
    public string? Level { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
}
