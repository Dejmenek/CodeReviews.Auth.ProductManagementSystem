namespace ProductManagementSystem.Domain;
public class Laptop : Computer
{
    public double ScreenSize { get; set; }
    public int BatteryLife { get; set; }
    public string WebcamQuality { get; set; } = string.Empty;
}
