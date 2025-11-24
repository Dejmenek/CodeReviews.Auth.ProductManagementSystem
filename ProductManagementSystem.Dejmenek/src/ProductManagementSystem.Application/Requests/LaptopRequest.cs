namespace ProductManagementSystem.Application.Requests;
public class LaptopRequest : ComputerRequest
{
    public double ScreenSize { get; set; }
    public int BatteryLife { get; set; }
    public string WebcamQuality { get; set; } = string.Empty;
}
