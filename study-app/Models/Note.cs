namespace StudyApp.Models;

public class Note
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Front { get; set; } = "";
    public string Back { get; set; } = "";
    public int Repetitions { get; set; } = 0;
    public int Interval { get; set; } = 1;
    public double EasinessFactor { get; set; } = 2.5;
    public DateTime NextReviewDate { get; set; } = DateTime.Today;
}
