using Boss.Models;


namespace Boss.Repositories;

public class WorkerRepository : IRepository<Worker>
{
    private readonly List<Worker> _workers = new();
    public WorkerRepository()
    {
        _workers = new List<Worker>();
    }

    public WorkerRepository(List<Worker> workers)
    {
        _workers = workers;
    }
    public void Add(Worker worker) => _workers.Add(worker);

    public void Update(Worker worker)
    {
        var existing = GetById(worker.Id);
        if (existing != null)
        {
            _workers.Remove(existing);
            _workers.Add(worker);
        }
    }

    public void Delete(int id) => _workers.RemoveAll(w => w.Id == id);

    public Worker GetById(int id) => _workers.Find(w => w.Id == id);

    public IEnumerable<Worker> GetAll() => _workers;



    public IEnumerable<Worker> SearchWorkers(string city = null, List<string> skills = null, string language = null)
    {
        return _workers.Where(worker =>
            (string.IsNullOrEmpty(city) || worker.City.Equals(city, StringComparison.OrdinalIgnoreCase)) &&
            (skills == null || worker.CVs.Any(cv => skills.All(skill => cv.Skills.Contains(skill)))) &&
            (string.IsNullOrEmpty(language) || worker.CVs.Any(cv => cv.Languages.Any(lang => lang.Name.Equals(language, StringComparison.OrdinalIgnoreCase))))
        );
    }










}
