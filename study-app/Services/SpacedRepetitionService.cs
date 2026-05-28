using StudyApp.Models;

namespace StudyApp.Services;

public static class SpacedRepetitionService
{
    public static void ApplyReview(Note note, int quality)
    {
        double newEF = note.EasinessFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02));
        newEF = Math.Max(1.3, newEF);
        note.EasinessFactor = newEF;

        if (quality < 3)
        {
            note.Repetitions = 0;
            note.Interval = 1;
        }
        else
        {
            if (note.Repetitions == 0)
            {
                note.Interval = 1;
                note.Repetitions = 1;
            }
            else if (note.Repetitions == 1)
            {
                note.Interval = 6;
                note.Repetitions = 2;
            }
            else
            {
                note.Interval = (int)Math.Round(note.Interval * note.EasinessFactor);
                note.Repetitions++;
            }
        }

        note.NextReviewDate = DateTime.Today.AddDays(note.Interval);
        DatabaseService.Instance.UpdateNoteReview(note);
    }
}
