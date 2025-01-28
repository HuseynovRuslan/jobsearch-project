using Boss.Models;

namespace Boss.Repositories;

public class EmployerRepository : IRepository<Employer>
{
    public EmployerRepository(List<Employer> employers)
    {
        _employers = employers;
    }

    private readonly List<Employer> _employers = new();

    public void Add(Employer employer) => _employers.Add(employer);

    public void Update(Employer employer)
    {
        var existing = GetById(employer.Id);
        if (existing != null)
        {
            _employers.Remove(existing);
            _employers.Add(employer);
        }
    }

    public void Delete(int id) => _employers.RemoveAll(e => e.Id == id);

    public Employer GetById(int id) => _employers.Find(e => e.Id == id);

    public IEnumerable<Employer> GetAll() => _employers;
    public IEnumerable<Vacancy> SearchVacancies(string title = null, string location = null, decimal? minSalary = null)
    {
        return _employers.SelectMany(e => e.Vacancies).Where(vacancy =>
            (string.IsNullOrEmpty(title) || vacancy.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(location) || vacancy.Location.Equals(location, StringComparison.OrdinalIgnoreCase)) &&
            (!minSalary.HasValue || vacancy.Salary >= minSalary)
        );
    }
}
