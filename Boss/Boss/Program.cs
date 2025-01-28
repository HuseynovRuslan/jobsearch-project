using Boss.Repositories;
using Boss.Services;
using System.Text;

class Program
{

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
       
        using (var unitOfWork = new UnitOfWork())
        {
            var workerRepo = unitOfWork.WorkerRepo;
            var employerRepo = unitOfWork.EmployerRepo;
            var authService = new AuthService(workerRepo, employerRepo);


            Boss.Actions.MenuHandler.DisplayMainMenu(authService, workerRepo, employerRepo, unitOfWork);
            
            unitOfWork.SaveData();
      
            
        }
    }
}
