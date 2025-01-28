namespace Boss.Models;

public class CV
{
    public int Id { get; set; }
    public string Specialty { get; set; }
    public string School { get; set; }
    public int UniAdmissionScore { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<WorkExperience> Companies { get; set; } = new();
    public List<Language> Languages { get; set; } = new();
    public bool HasDistinctionDiploma { get; set; }
    public string GitLink { get; set; }
    public string LinkedIn { get; set; }
}
