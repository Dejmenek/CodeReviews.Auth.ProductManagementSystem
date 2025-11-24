namespace ProductManagementSystem.Domain;
public class Desktop : Computer
{
    public CaseType CaseType { get; set; }
}

public enum CaseType
{
    MidTower,
    MiniTower,
    SmallFormFactor,
    FullTower
}
