namespace Boss.Models;

public class Employer:User
{

    public List<Vacancy> Vacancies { get; set; } = new();
}
