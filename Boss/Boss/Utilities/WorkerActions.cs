using Boss.Models;
using Boss.Repositories;
using Boss.Utilities;
namespace Boss.Actions
{
    public static class WorkerActions
    {
        public static void WorkerPanel(Worker worker, WorkerRepository workerRepo, EmployerRepository employerRepo, UnitOfWork unitOfWork)
        {
            while (true)
            {
                Console.WriteLine("\nİşçi Panelinə Xoş Gəlmisiniz!");
                Console.WriteLine("1. CV elave et");
                Console.WriteLine("2. CV-ni goster");
                Console.WriteLine("3. Vakansiyalara bax");
                Console.WriteLine("4. Vakansiya Axtarışı");
                Console.WriteLine("5. CV-ni Redaktə Etmək");
                Console.WriteLine("6. Çıxış");
                Console.Write("Seçiminizi daxil edin: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCV(worker,unitOfWork);
                        break;

                    case "2":
                        ShowCVs(worker);
                        break;

                    case "3":
                        ViewVacancies(employerRepo);
                        break;

               
                    case "4":
                        SearchVacancies(employerRepo);
                        break;
                    case "5":
                        EditCV(worker, unitOfWork);
                        break;
                    case "6":
                        Console.WriteLine("İşçi panelindən çıxış edilir...");
                        return;
                    default:
                        Console.WriteLine("Yanlış seçim! Yenidən cəhd edin.");
                        break;
                }
            }
        }

        private static void ViewVacancies(EmployerRepository employerRepo)
        {
            Console.WriteLine("\nVakansiyalar:");

            var allVacancies = employerRepo.GetAll()
                .SelectMany(employer => employer.Vacancies)
                .ToList();

            if (!allVacancies.Any())
            {
                Console.WriteLine("Hazırda heç bir vakansiya mövcud deyil.");
                return;
            }

            foreach (var vacancy in allVacancies)
            {
                Console.WriteLine($"- Ad: {vacancy.Title}");
                Console.WriteLine($"  Təsvir: {vacancy.Description}");
                Console.WriteLine($"  Tələblər: {vacancy.Requirements}");
                Console.WriteLine($"  Yer: {vacancy.Location}");
                Console.WriteLine($"  Maaş: {vacancy.Salary:C}");
                Console.WriteLine($"  Email: {vacancy.Email:C}");
                Console.WriteLine(new string('-', 40));
            }
        }

        private static void AddCV(Worker worker, UnitOfWork unitOfWork)
        {
            var newCV = new CV();

            Console.Write("Ixtisas: ");
            newCV.Specialty = Console.ReadLine();

            Console.Write("Tehsil aldiginiz universitetin adini daxil edin: ");
            newCV.School = Console.ReadLine();

            Console.Write("Universitet qebul balı: ");
            if (int.TryParse(Console.ReadLine(), out int uniScore))
            {
                newCV.UniAdmissionScore = uniScore;
            }
            else
            {
                Console.WriteLine("Qebul balı düzgün daxil edilmedi!");
                return;
            }

            Console.Write("Bacarıqlar (vergüllə ayrılmış): ");
            var skillsInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(skillsInput))
            {
                newCV.Skills = skillsInput.Split(',').Select(skill => skill.Trim()).ToList();
            }

            Console.WriteLine("İş təcrübəsi əlavə etmək istəyirsiniz? (beli/xeyr)");
            var addExperience = Console.ReadLine()?.ToLower() == "beli";
            while (addExperience)
            {
                var experience = new WorkExperience();
                Console.Write("Şirkət adı: ");
                experience.CompanyName = Console.ReadLine();

                Console.Write("Başlama tarixi (YYYY-MM-DD): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    experience.StartDate = startDate;
                }
                else
                {
                    Console.WriteLine("Tarix düzgün daxil edilmədi!");
                    continue;
                }

                Console.Write("Bitmə tarixi (YYYY-MM-DD): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    experience.EndDate = endDate;
                }
                else
                {
                    Console.WriteLine("Tarix düzgün daxil edilmədi!");
                    continue;
                }

                newCV.Companies.Add(experience);

                Console.WriteLine("Daha bir iş təcrübəsi əlavə etmək istəyirsiniz? (beli/xeyr)");
                addExperience = Console.ReadLine()?.ToLower() == "beli";
            }

            Console.WriteLine("Bildiyiniz xarici dilləri əlavə edin:");
            Console.Write("Dil adı (meselen, İngilis dili): ");
            var languageName = Console.ReadLine();

            Console.Write("Dil səviyyəsi (meselen, Başlanğıc, Orta, Yüksek): ");
            var languageLevel = Console.ReadLine();

            if (!string.IsNullOrEmpty(languageName) && !string.IsNullOrEmpty(languageLevel))
            {
                newCV.Languages.Add(new Language
                {
                    Name = languageName,
                    Level = languageLevel
                });
            }

            Console.Write("Fərqlənmə diplomu var mı? (beli/xeyr): ");
            newCV.HasDistinctionDiploma = Console.ReadLine()?.ToLower() == "beli";

            
            Console.Write("GitHub bağlantınız varmı? (beli/xeyr): ");
            if (Console.ReadLine()?.ToLower() == "beli")
            {
                Console.Write("GitHub Link: ");
                newCV.GitLink = Console.ReadLine();
            }

            
            Console.Write("LinkedIn bağlantınız varmı? (beli/xeyr): ");
            if (Console.ReadLine()?.ToLower() == "beli")
            {
                Console.Write("LinkedIn Link: ");
                newCV.LinkedIn = Console.ReadLine();
            }

            newCV.Id = worker.CVs.Count + 1;
            worker.CVs.Add(newCV);
            unitOfWork.SaveData();

            Console.WriteLine("CV uğurla əlavə edildi!");
            
         
            
            foreach (var cv in worker.CVs)
            {
                Console.WriteLine($"CV: {cv.Specialty}, Skills Count: {cv.Skills.Count}");
            }
        }
        private static void ShowCVs(Worker worker)
        {
            Console.WriteLine("\nSizin CV-ləriniz:");

            if (!worker.CVs.Any())
            {
                Console.WriteLine("Hazırda heç bir CV mövcud deyil.");
                return;
            }

            foreach (var cv in worker.CVs)
            {
                Console.WriteLine($"- Ixtisas: {cv.Specialty}");
                Console.WriteLine($"  Üniversitet: {cv.School}");
                Console.WriteLine($"  Qəbul balı: {cv.UniAdmissionScore}");
                Console.WriteLine($"  Bacarıqlar: {string.Join(", ", cv.Skills)}");

                Console.WriteLine($"  Xarici Dillər: {string.Join(", ", cv.Languages.Select(lang => $"{lang.Name} ({lang.Level})"))}");

                Console.WriteLine($"  İş Təcrübəsi:");
                foreach (var company in cv.Companies)
                {
                    Console.WriteLine($"    Şirkət: {company.CompanyName}, Tarix: {company.StartDate.ToShortDateString()} - {company.EndDate.ToShortDateString()}");
                }

                Console.WriteLine($"  Fərqlənmə Diplomu: {(cv.HasDistinctionDiploma ? "Bəli" : "Xeyr")}");
                Console.WriteLine($"  GitHub: {cv.GitLink}");
                Console.WriteLine($"  LinkedIn: {cv.LinkedIn}");
                Console.WriteLine(new string('-', 20));
            }
        }
        private static void SearchVacancies(EmployerRepository employerRepo)
        {
            Console.WriteLine("\nVakansiya Axtarışı:");

            try
            {
               
                Console.Write("Vakansiya başlığı daxil edin (boş buraxın): ");
                string title = Console.ReadLine()?.ToLower(); 

                
                Console.Write("Şəhər daxil edin (boş buraxın): ");
                string location = Console.ReadLine()?.ToLower(); 

               
                Console.Write("Minimum maaş daxil edin (boş buraxın): ");
                decimal? minSalary = null;
                try
                {
                    string salaryInput = Console.ReadLine();
                    if (!string.IsNullOrEmpty(salaryInput))
                    {
                        minSalary = decimal.Parse(salaryInput); 
                        if (minSalary < 0) throw new ArgumentOutOfRangeException("Minimum maaş mənfi ola bilməz.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Düzgün maaş daxil edin (məsələn, 1500).");
                    return;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
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
                    Console.WriteLine($"Tələblər: {vacancy.Requirements}");
                    Console.WriteLine($"Şəhər: {vacancy.Location}");
                    Console.WriteLine($"Maaş: {vacancy.Salary:F2} ₼");
                    Console.WriteLine($"Əlaqə Email: {vacancy.Email}");
                    Console.WriteLine(new string('-', 40));
                }
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.Handle(ex, "Vakansiya Axtarışı");
            }
        }
        private static void EditCV(Worker worker, UnitOfWork unitOfWork)
        {
            try
            {
                if (!worker.CVs.Any())
                {
                    Console.WriteLine("Sizin heç bir CV-niz mövcud deyil.");
                    return;
                }

                Console.WriteLine("\nSizin CV-ləriniz:");
                for (int i = 0; i < worker.CVs.Count; i++)
                {
                    var cv = worker.CVs[i];
                    Console.WriteLine($"{i + 1}. Ixtisas: {cv.Specialty}, Universitet: {cv.School}");
                }

                Console.Write("\nDəyişmək istədiyiniz CV-nin nömrəsini daxil edin: ");
                if (!int.TryParse(Console.ReadLine(), out int cvIndex) || cvIndex < 1 || cvIndex > worker.CVs.Count)
                {
                    Console.WriteLine("Yanlış seçim! Yenidən cəhd edin.");
                    return;
                }

                var selectedCV = worker.CVs[cvIndex - 1];

                
                Console.WriteLine($"Hazırkı ixtisas: {selectedCV.Specialty}");
                Console.Write("Yeni ixtisas (boş buraxdınsa dəyişilməyəcək): ");
                string newSpecialty = Console.ReadLine();
                if (!string.IsNullOrEmpty(newSpecialty))
                {
                    selectedCV.Specialty = newSpecialty;
                }

                Console.WriteLine($"Hazırkı universitet: {selectedCV.School}");
                Console.Write("Yeni universitet (boş buraxdınsa dəyişilməyəcək): ");
                string newSchool = Console.ReadLine();
                if (!string.IsNullOrEmpty(newSchool))
                {
                    selectedCV.School = newSchool;
                }

                Console.WriteLine($"Hazırkı qəbul balı: {selectedCV.UniAdmissionScore}");
                Console.Write("Yeni qəbul balı (boş buraxdınsa dəyişilməyəcək): ");
                string newScoreInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(newScoreInput))
                {
                    if (!int.TryParse(newScoreInput, out int newScore) || newScore < 0)
                    {
                        Console.WriteLine("Düzgün qəbul balı daxil edin.");
                        return;
                    }
                    selectedCV.UniAdmissionScore = newScore;
                }

                Console.WriteLine($"Hazırkı bacarıqlar: {string.Join(", ", selectedCV.Skills)}");
                Console.Write("Yeni bacarıqlar (vergüllə ayrılmış, boş buraxdınsa dəyişilməyəcək): ");
                string newSkillsInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(newSkillsInput))
                {
                    selectedCV.Skills = newSkillsInput.Split(',').Select(skill => skill.Trim()).ToList();
                }

                Console.WriteLine($"Hazırkı GitHub link: {selectedCV.GitLink}");
                Console.Write("Yeni GitHub link (boş buraxdınsa dəyişilməyəcək): ");
                string newGitLink = Console.ReadLine();
                if (!string.IsNullOrEmpty(newGitLink))
                {
                    selectedCV.GitLink = newGitLink;
                }

                Console.WriteLine($"Hazırkı LinkedIn link: {selectedCV.LinkedIn}");
                Console.Write("Yeni LinkedIn link (boş buraxdınsa dəyişilməyəcək): ");
                string newLinkedIn = Console.ReadLine();
                if (!string.IsNullOrEmpty(newLinkedIn))
                {
                    selectedCV.LinkedIn = newLinkedIn;
                }

                Console.WriteLine("CV uğurla redaktə edildi!");
                unitOfWork.SaveData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "CV Redaktəsi");
                Console.WriteLine("CV-ni redaktə edərkən xəta baş verdi.");
            }
        }
        private static void DeleteCV(Worker worker, UnitOfWork unitOfWork)
        {
            try
            {
                if (!worker.CVs.Any())
                {
                    Console.WriteLine("Sizin heç bir CV-niz mövcud deyil.");
                    return;
                }

                Console.WriteLine("\nSizin CV-ləriniz:");
                for (int i = 0; i < worker.CVs.Count; i++)
                {
                    var cv = worker.CVs[i];
                    Console.WriteLine($"{i + 1}. Ixtisas: {cv.Specialty}, Universitet: {cv.School}");
                }

                Console.Write("\nSilinmək istədiyiniz CV-nin nömrəsini daxil edin: ");
                if (!int.TryParse(Console.ReadLine(), out int cvIndex) || cvIndex < 1 || cvIndex > worker.CVs.Count)
                {
                    Console.WriteLine("Yanlış seçim! Yenidən cəhd edin.");
                    return;
                }

                var selectedCV = worker.CVs[cvIndex - 1];

                Console.WriteLine($"\nSilinmək istədiyiniz CV: {selectedCV.Specialty}");
                Console.Write("Əminsinizmi? (bəli/xeyr): ");
                string confirmation = Console.ReadLine()?.ToLower();

                if (confirmation == "bəli")
                {
                    worker.CVs.Remove(selectedCV);
                    Console.WriteLine("CV uğurla silindi!");
                    unitOfWork.SaveData();
                }
                else
                {
                    Console.WriteLine("Silinmə prosesi ləğv edildi.");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "CV Silmə");
                Console.WriteLine("CV-ni silərkən xəta baş verdi.");
            }
        }






    }
}
