namespace StudyApp.Models;

public class Module
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Color { get; set; } = "#007AFF";
    public int DueCount { get; set; }
    public int TotalCount { get; set; }
}
