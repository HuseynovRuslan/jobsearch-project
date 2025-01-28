using Newtonsoft.Json;

public static class ExceptionHandler
{
    private static readonly string _logFilePath = "error_log.json";

    public static void Handle(Exception ex, string context = "General")
    {
       
        var logEntry = new
        {
            Context = context,
            Exception = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            Date = DateTime.Now
        };

        File.AppendAllText(_logFilePath, JsonConvert.SerializeObject(logEntry, Formatting.Indented) + Environment.NewLine);

       
        Console.WriteLine($"Xəta baş verdi: {ex.Message}. Ətraflı məlumat log faylında saxlanıldı.");
    }
    public static void DisplayErrorLogs()
    {
        if (File.Exists(_logFilePath))
        {
            var logs = File.ReadAllText(_logFilePath);
            Console.WriteLine("Xəta logları:");
            Console.WriteLine(logs);
        }
        else
        {
            Console.WriteLine("Log faylı mövcud deyil.");
        }
    }
}
