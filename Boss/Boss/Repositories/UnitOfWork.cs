using Boss.Models;
using Newtonsoft.Json;
using System.Text;
namespace Boss.Repositories;

public class UnitOfWork : IDisposable
{
    private readonly string _dataFilePath = "data.json";
    private readonly string _logFilePath = "log.json";

    public List<Worker> Workers { get; private set; } = new List<Worker>();
    public List<Employer> Employers { get; private set; } = new List<Employer>();

    public WorkerRepository WorkerRepo { get; private set; }
    public EmployerRepository EmployerRepo { get; private set; }

    public UnitOfWork()
    {
        LoadData();
        WorkerRepo = new WorkerRepository(Workers);
        EmployerRepo = new EmployerRepository(Employers);

    }

    public void AddLog(string log)
    {
        var logEntry = $"{DateTime.Now}: {log}";
        File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
    }

    public void SaveData()
    {
        try
        {
            var data = new { 

                Workers, 
                Employers,
                Cvs = Workers.SelectMany(w=> w.CVs).ToList(),
                Vacancies = Employers.SelectMany(e=> e.Vacancies).ToList() 
            };

            File.WriteAllText(_dataFilePath, JsonConvert.SerializeObject(data, Formatting.Indented), Encoding.UTF8);

            AddLog("Data saved successfully.");
          


        }

        catch (UnauthorizedAccessException ex)
        {
           
            AddLog($"Permission error while saving data: {ex.Message}");
        }
        catch (IOException ex)
        {
           
            AddLog($"I/O error while saving data: {ex.Message}");
        }
        catch (Exception ex)
        {
           AddLog($"Error saving data: {ex.Message}");
            Console.WriteLine($"Məlumat yazılarkən xəta baş verdi: {ex.Message}");



        }
    }

    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(_dataFilePath));
                Workers = JsonConvert.DeserializeObject<List<Worker>>(data.Workers.ToString());
                Employers = JsonConvert.DeserializeObject<List<Employer>>(data.Employers.ToString());
                var allCVs = JsonConvert.DeserializeObject<List<CV>>(data.CVs.ToString());

                foreach (var worker in Workers) {



                    Func<CV, bool> predicate = cv => cv.Id == worker.Id;
                    worker.CVs = allCVs.Where(predicate).ToList();


                }
                foreach (var worker in Workers)
                {
                    Console.WriteLine($"Worker: {worker.Name}, CV Count: {worker.CVs.Count}");
                }
                var allVacancies = JsonConvert.DeserializeObject<List<Vacancy>>(data.Vacancies.ToString());
                foreach (var employer in Employers)
                {
                    Func<Vacancy, bool> predicate = v => v.EmployerId == employer.Id;
                    employer.Vacancies = allVacancies.Where(predicate).ToList();
                }











                AddLog("Data loaded successfully.");
            }


            catch (JsonSerializationException ex)
            {
               
                AddLog($"JSON deserialization error: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                
                AddLog($"File not found: {ex.Message}");
            }






            catch (Exception ex)
            {
                AddLog($"Error loading data: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        SaveData();
    }
}
