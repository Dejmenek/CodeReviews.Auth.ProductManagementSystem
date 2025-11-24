using ProductManagementSystem.Domain;

namespace ProductManagementSystem.Application.Requests;
public class DesktopRequest : ComputerRequest
{
    public CaseType CaseType { get; set; }
}
