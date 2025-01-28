using Boss.Models;
using Boss.Repositories;
using System.Text.RegularExpressions;
namespace Boss.Actions;
using Boss.Utilities;
public static class EmployerActions
{
    public static void EmployerPanel(Employer employer, EmployerRepository employerRepo, WorkerRepository workerRepo, UnitOfWork unitOfWork)
    {
        while (true)
        {
            Console.WriteLine("\nİşegötürən Paneline Xoş Gelmisiniz!");
            Console.WriteLine("1. Vakansiya elavə et");
            Console.WriteLine("2. Öz vakansiyalarına bax");
            Console.WriteLine("3. Ümumi vakansiyalara bax");
            Console.WriteLine("4. Ümumi Vakansiya Axtarışı");
            Console.WriteLine("5. Ümumi CV-lərə bax");
            Console.WriteLine("6. Vakansiyanı redaktə et");
            Console.WriteLine("7. Vakansiyanı sil");
            Console.WriteLine("8. CV axtarışı");

            Console.WriteLine("9. Çıxış");
            Console.Write("Seçiminizi daxil edin: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddVacancy(employer, unitOfWork);
                    break;

                case "2":
                    ShowOwnVacancies(employer);
                    break;

                case "3":
                    ShowAllVacancies(employerRepo);
                    break;
                case "4":
                    SearchVacancies(employerRepo);
                    break;

                case "5":
                    ViewAllCVs(workerRepo);
                    break;

                case "6":
                    EditVacancy(employer, unitOfWork);
                    break;
                case "7":
                    DeleteVacancy(employer, unitOfWork);
                    break;
                case "8":
                    SearchCVs(workerRepo);
                    break;

                case "9":
                    Console.WriteLine("İşegötüren panelindən çıxıldı");
                    return;

                default:
                    Console.WriteLine("Yanlış seçim! Yenidfn cehd edin.");
                    break;
            }
        }
    }

    private static void AddVacancy(Employer employer, UnitOfWork unitOfWork)
    {
        try
        {
            var vacancy = new Vacancy();

            Console.Write("Vakansiya adı: ");
            vacancy.Title = Console.ReadLine();

            Console.Write("Təsvir: ");
            vacancy.Description = Console.ReadLine();

            Console.Write("Tələblər: ");
            vacancy.Requirements = Console.ReadLine();

            Console.Write("Şəhər: ");
            vacancy.Location = Console.ReadLine();

          
            Console.Write("Maaş: ");
            try
            {
                if (!decimal.TryParse(Console.ReadLine(), out decimal salary))
                {
                    throw new FormatException("Düzgün maaş daxil edin (məsələn, 1500).");
                }

                if (salary < 0)
                {
                    throw new ArgumentOutOfRangeException("Maaş mənfi ola bilməz.");
                }

                vacancy.Salary = salary;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentOutOfRangeException)
            {
                Console.WriteLine(ex.Message);
                return;
            }

           
            Console.Write("Əlaqə Email: ");
            string email = Console.ReadLine();
            if (!EmailValidator.IsValidEmail(email)) 
            {
                Console.WriteLine("Düzgün formatda email daxil edin (məsələn, example@domain.com).");
                return;
            }
            vacancy.Email = email;

            
            vacancy.Id = employer.Vacancies.Count + 1;
            vacancy.EmployerId = employer.Id;

            employer.Vacancies.Add(vacancy);
            Console.WriteLine("Vakansiya uğurla əlavə edildi!");

           
            unitOfWork.SaveData();
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex, "Vakansiya Əlavə Edilməsi");
            Console.WriteLine("Vakansiya əlavə edilərkən xəta baş verdi.");
        }
    }

    private static void DeleteVacancy(Employer employer, UnitOfWork unitOfWork)
    {
        try
        {
            if (!employer.Vacancies.Any())
            {
                Console.WriteLine("Sizin heç bir vakansiyanız mövcud deyil.");
                return;
            }

            Console.WriteLine("\nSizin Vakansiyalarınız:");
            for (int i = 0; i < employer.Vacancies.Count; i++)
            {
                var vacancy = employer.Vacancies[i];
                Console.WriteLine($"{i + 1}. Başlıq: {vacancy.Title}, Maaş: {vacancy.Salary:F2} ₼, Şəhər: {vacancy.Location}");
            }

            Console.Write("\nSilmək istədiyiniz vakansiyanın nömrəsini daxil edin: ");
            if (!int.TryParse(Console.ReadLine(), out int vacancyIndex) || vacancyIndex < 1 || vacancyIndex > employer.Vacancies.Count)
            {
                Console.WriteLine("Yanlış seçim! Yenidən cəhd edin.");
                return;
            }

            var selectedVacancy = employer.Vacancies[vacancyIndex - 1];

            Console.WriteLine($"\nSilmək istədiyiniz vakansiya: {selectedVacancy.Title}");
            Console.Write("Əminsinizmi? (beli/xeyr): ");
            string confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "beli")
            {
                employer.Vacancies.Remove(selectedVacancy);
                Console.WriteLine("Vakansiya uğurla silindi!");
                unitOfWork.SaveData();
            }
            else
            {
                Console.WriteLine("Silinmə prosesi ləğv edildi.");
            }
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex, "Vakansiya Silmə");
            Console.WriteLine("Vakansiyanı silərkən xəta baş verdi.");
        }
    }


    private static void ShowOwnVacancies(Employer employer)
    {
        Console.WriteLine("\nSizin Vakansiyalarınız:");
        foreach (var vacancy in employer.Vacancies)
        {
            Console.WriteLine($"- Ad: {vacancy.Title}");
            Console.WriteLine($"  Tesvir: {vacancy.Description}");
            Console.WriteLine($"  Telebler: {vacancy.Requirements}");
            Console.WriteLine($"  Yer: {vacancy.Location}");
            Console.WriteLine($"  Maaş: {vacancy.Salary:F2} ₼");
            Console.WriteLine($"  Elaqe Email: {vacancy.Email}");
            Console.WriteLine(new string('-', 20));
        }
    }
    private static void EditVacancy(Employer employer, UnitOfWork unitOfWork)
    {
        try
        {
            if (!employer.Vacancies.Any())
            {
                Console.WriteLine("Sizin heç bir vakansiyanız mövcud deyil.");
                return;
            }

            Console.WriteLine("\nSizin Vakansiyalarınız:");
            for (int i = 0; i < employer.Vacancies.Count; i++)
            {
                var vacancy = employer.Vacancies[i];
                Console.WriteLine($"{i + 1}. Başlıq: {vacancy.Title}, Maaş: {vacancy.Salary:F2} ₼, Şəhər: {vacancy.Location}");
            }

            Console.Write("\nDəyişmək istədiyiniz vakansiyanın nömrəsini daxil edin: ");
            if (!int.TryParse(Console.ReadLine(), out int vacancyIndex) || vacancyIndex < 1 || vacancyIndex > employer.Vacancies.Count)
            {
                Console.WriteLine("Yanlış seçim! Yenidən cəhd edin.");
                return;
            }

            var selectedVacancy = employer.Vacancies[vacancyIndex - 1];

            Console.WriteLine($"Hazırkı başlıq: {selectedVacancy.Title}");
            Console.Write("Yeni başlıq (boş buraxdınsa dəyişilməyəcək): ");
            string newTitle = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTitle))
            {
                selectedVacancy.Title = newTitle;
            }

            Console.WriteLine($"Hazırkı təsvir: {selectedVacancy.Description}");
            Console.Write("Yeni təsvir (boş buraxdınsa dəyişilməyəcək): ");
            string newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription))
            {
                selectedVacancy.Description = newDescription;
            }

            Console.WriteLine($"Hazırkı tələblər: {selectedVacancy.Requirements}");
            Console.Write("Yeni tələblər (boş buraxdınsa dəyişilməyəcək): ");
            string newRequirements = Console.ReadLine();
            if (!string.IsNullOrEmpty(newRequirements))
            {
                selectedVacancy.Requirements = newRequirements;
            }

            Console.WriteLine($"Hazırkı şəhər: {selectedVacancy.Location}");
            Console.Write("Yeni şəhər (boş buraxdınsa dəyişilməyəcək): ");
            string newLocation = Console.ReadLine();
            if (!string.IsNullOrEmpty(newLocation))
            {
                selectedVacancy.Location = newLocation;
            }

            Console.WriteLine($"Hazırkı maaş: {selectedVacancy.Salary:F2} ₼");
            Console.Write("Yeni maaş (boş buraxdınsa dəyişilməyəcək): ");
            string newSalaryInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(newSalaryInput))
            {
                if (!decimal.TryParse(newSalaryInput, out decimal newSalary) || newSalary < 0)
                {
                    Console.WriteLine("Düzgün maaş daxil edin.");
                    return;
                }
                selectedVacancy.Salary = newSalary;
            }

            Console.WriteLine($"Hazırkı əlaqə email: {selectedVacancy.Email}");
            Console.Write("Yeni əlaqə email (boş buraxdınsa dəyişilməyəcək): ");
            string newEmail = Console.ReadLine();
            if (!string.IsNullOrEmpty(newEmail))
            {
                if (!EmailValidator.IsValidEmail(newEmail))
                {
                    Console.WriteLine("Düzgün formatda email daxil edin.");
                    return;
                }
                selectedVacancy.Email = newEmail;
            }

            Console.WriteLine("Vakansiya uğurla redaktə edildi!");
            unitOfWork.SaveData();
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex, "Vakansiya Redaktəsi");
            Console.WriteLine("Vakansiyanı redaktə edərkən xəta baş verdi.");
        }
    }


    private static void ShowAllVacancies(EmployerRepository employerRepo)
    {
        try
        {
            Console.WriteLine("\nÜmumi Vakansiyalar:");

            var allVacancies = employerRepo.GetAll()
                .SelectMany(employer => employer.Vacancies)
                .ToList();

            if (!allVacancies.Any())
            {
                Console.WriteLine("Ümumi vakansiyalar mövcud deyil.");
                return;
            }

            foreach (var vacancy in allVacancies)
            {
                Console.WriteLine($"Başlıq: {vacancy.Title}");
                Console.WriteLine($"Təsvir: {vacancy.Description}");
                Console.WriteLine($"Tələblər: {vacancy.Requirements}");
                Console.WriteLine($"Şəhər: {vacancy.Location}");
                Console.WriteLine($"Maaş: {vacancy.Salary:F2} ₼");
                Console.WriteLine($"Əlaqə Email: {vacancy.Email}");
                Console.WriteLine(new string('-', 40));
            }
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex, "Ümumi Vakansiyalara Baxış");
            Console.WriteLine("Vakansiyaları göstərərkən xəta baş verdi.");
        }
    }
    private static void ViewAllCVs(WorkerRepository workerRepo)
    {
        try
        {
            Console.WriteLine("\nÜmumi CV-lər:");

            
            var allCVs = workerRepo.GetAll()
                .SelectMany(worker => worker.CVs)
                .ToList();

            if (!allCVs.Any())
            {
                Console.WriteLine("Hazırda heç bir CV mövcud deyil.");
                return;
            }

           
            foreach (var cv in allCVs)
            {
                Console.WriteLine($"Ixtisas: {cv.Specialty}");
                Console.WriteLine($"Oxuduğu Universitet: {cv.School}");
                Console.WriteLine($"Qəbul Balı: {cv.UniAdmissionScore}");
                Console.WriteLine($"Bacarıqlar: {string.Join(", ", cv.Skills)}");
                Console.WriteLine($"Bildiyi Dillər: {string.Join(", ", cv.Languages.Select(lang => $"{lang.Name} ({lang.Level})"))}");
                Console.WriteLine($"İş Təcrübəsi:");
                foreach (var company in cv.Companies)
                {
                    Console.WriteLine($"  Şirkət: {company.CompanyName}, {company.StartDate.ToShortDateString()} - {company.EndDate.ToShortDateString()}");
                }
                Console.WriteLine($"Fərqlənmə Diplomu: {(cv.HasDistinctionDiploma ? "Bəli" : "Xeyr")}");
                Console.WriteLine($"GitHub: {cv.GitLink}");
                Console.WriteLine($"LinkedIn: {cv.LinkedIn}");
                Console.WriteLine(new string('-', 40));
            }
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex, "Ümumi CV-lərə Baxış");
            Console.WriteLine("CV-lərə baxış zamanı xəta baş verdi.");
        }
    }

    private static void SearchVacancies(EmployerRepository employerRepo)
    {
        Console.WriteLine("\nVakansiya Axtarışı:");

        Console.Write("Başlıq daxil edin (boş buraxın): ");
        string title = Console.ReadLine();

        Console.Write("Məkan daxil edin (boş buraxın): ");
        string location = Console.ReadLine();

        Console.Write("Minimum maaş daxil edin (boş buraxın): ");
        decimal? minSalary = null;
        if (decimal.TryParse(Console.ReadLine(), out decimal salary))
        {
            minSalary = salary;
        }

        var filteredVacancies = employerRepo.SearchVacancies(title, location, minSalary).ToList();

        if (!filteredVacancies.Any())
        {
            Console.WriteLine("Heç bir vakansiya tapılmadı.");
            return;
        }

        Console.WriteLine("\nAxtarış Nəticələri:");
        foreach (var vacancy in filteredVacancies)
        {
            Console.WriteLine($"Başlıq: {vacancy.Title}");
            Console.WriteLine($"Təsvir: {vacancy.Description}");
            Console.WriteLine($"Məkan: {vacancy.Location}");
            Console.WriteLine($"Maaş: {vacancy.Salary:F2} ₼");
            Console.WriteLine(new string('-', 40));
        }
    }
    private static void SearchCVs(WorkerRepository workerRepo)
    {
        try
        {
            Console.WriteLine("\nCV Axtarışı:");

          
            Console.Write("İxtisas (boş buraxdınsa axtarış edilməyəcək): ");
            string specialty = Console.ReadLine()?.ToLower();

           
            Console.Write("Universitet (boş buraxdınsa axtarış edilməyəcək): ");
            string school = Console.ReadLine()?.ToLower();

          
            Console.Write("Minimum qəbul balı (boş buraxdınsa axtarış edilməyəcək): ");
            string scoreInput = Console.ReadLine();
            int? minScore = null;
            if (!string.IsNullOrEmpty(scoreInput) && int.TryParse(scoreInput, out int score))
            {
                minScore = score;
            }

          
            var filteredCVs = workerRepo.GetAll()
                .SelectMany(worker => worker.CVs)
                .Where(cv =>
                    (string.IsNullOrEmpty(specialty) || cv.Specialty.ToLower().Contains(specialty)) &&
                    (string.IsNullOrEmpty(school) || cv.School.ToLower().Contains(school)) &&
                    (!minScore.HasValue || cv.UniAdmissionScore >= minScore))
                .ToList();

            if (!filteredCVs.Any())
            {
                Console.WriteLine("Heç bir uyğun CV tapılmadı.");
                return;
            }

           
            Console.WriteLine("\nAxtarış Nəticələri:");
            foreach (var cv in filteredCVs)
            {
                Console.WriteLine($"Ixtisas: {cv.Specialty}");
                Console.WriteLine($"Universitet: {cv.School}");
                Console.WriteLine($"Qəbul Balı: {cv.UniAdmissionScore}");
                Console.WriteLine($"Bacarıqlar: {string.Join(", ", cv.Skills)}");
                Console.WriteLine($"İş Təcrübəsi:");
                foreach (var company in cv.Companies)
                {
                    Console.WriteLine($"  Şirkət: {company.CompanyName}, {company.StartDate.ToShortDateString()} - {company.EndDate.ToShortDateString()}");
                }
                Console.WriteLine($"Dil Bacarıqları: {string.Join(", ", cv.Languages.Select(lang => $"{lang.Name} ({lang.Level})"))}");
                Console.WriteLine($"Fərqlənmə Diplomu: {(cv.HasDistinctionDiploma ? "Bəli" : "Xeyr")}");
                Console.WriteLine($"GitHub: {cv.GitLink}");
                Console.WriteLine($"LinkedIn: {cv.LinkedIn}");
                Console.WriteLine(new string('-', 40));
            }
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex, "CV Axtarışı");
            Console.WriteLine("CV axtarışı zamanı xəta baş verdi.");
        }
    }



}
