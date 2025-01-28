using Boss.Models;
using Boss.Repositories;
using Boss.Services;
using Boss.Utilities;
using System.Globalization;

namespace Boss.Actions;

public static class MenuHandler
{
    public static void DisplayMainMenu(AuthService authService, WorkerRepository workerRepo, EmployerRepository employerRepo, UnitOfWork unitOfWork)
    {
        while (true)
        {
            Console.WriteLine("\nXoş Gelmisiniz!");
            Console.WriteLine("1. Qeydiyyat");
            Console.WriteLine("2. Daxil ol");
            Console.WriteLine("3. Çıxış");
            Console.Write("Seçiminizi daxil edin (1-3): ");
            var choice = Console.ReadLine();
         

            switch (choice)
            {
                case "1":
                    HandleRegistration(authService, unitOfWork);
                    break;

                case "2":
                    HandleLogin(authService, workerRepo, employerRepo, unitOfWork);
                    break;

                case "3":
                    Console.WriteLine("Sistemdən çıxış edilir. Sağ olun!");
                    return;

                default:
                    Console.WriteLine("Yanlış seçim! Yenidən cəhd edin.");
                    break;
            }
        }
    }

    private static void HandleRegistration(AuthService authService, UnitOfWork unitOfWork)
    {
        Console.WriteLine("\nQeydiyyat:");
        Console.WriteLine("1. İşçi olaraq qeydiyyat");
        Console.WriteLine("2. İşəgötürən olaraq qeydiyyat");
        Console.Write("Seçiminizi daxil edin (1-2): ");
    
        string choice;
        while (true)
        {
            Console.Write("Seçiminizi daxil edin (1-2): ");
            choice = Console.ReadLine();

            if (choice == "1" || choice == "2")
            {
                break; 
            }
            else
            {
                Console.WriteLine("Yanlış seçim! Zəhmət olmasa 1 və ya 2 daxil edin.");
            }
        }
        
        string username;
        while (true)
        {
            Console.Write("İstifadəçi adı: ");
            username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("İstifadəçi adı boş ola bilməz. Yenidən daxil edin.");
            }
            else if (username.Length < 3)
            {
                Console.WriteLine("İstifadəçi adı ən azı 3 simvol uzunluğunda olmalıdır.");
            }
            else
            {
                break;
            }
        }




        string password;
        while (true)
        {
            Console.Write("Parol: ");
            password = Console.ReadLine();
            if (PasswordValidator.IsStrongPassword(password))
            {
                break;
            }
            Console.WriteLine("Parol güclü deyil! Ən azı bir böyük hərf, bir rəqəm, bir xüsusi simvol daxil etməli və 8 simvoldan uzun olmalıdır.");
        }
        string email;
        while (true)
        {
            Console.Write("E-poçt: ");
            email = Console.ReadLine();
            if (!authService.IsEmailRegistered(email) && email.Contains("@") && email.Contains("."))
            {
                break;
            }
            Console.WriteLine("Bu email artıq qeydiyyatdan keçib və ya düzgün deyil. Yenidən daxil edin.");
        }





        string name, surname;
        while (true)
        {
            Console.Write("Ad: ");
            name = Console.ReadLine();
            Console.Write("Soyad: ");
            surname = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname))
            {
                Console.WriteLine("Ad və soyad boş ola bilməz. Yenidən daxil edin.");
            }
            else if (name.Length < 2 || surname.Length < 2)
            {
                Console.WriteLine("Ad və soyad ən azı 2 simvol uzunluğunda olmalıdır.");
            }
            else
            {
                break;
            }
        }


            int age;
        while (true)
        {
            Console.Write("Yaş: ");
            if (int.TryParse(Console.ReadLine(), out age) && age > 0)
            {
                break;
            }
            Console.WriteLine("Düzgün yaş daxil edin.");
        }
        Console.Write("Şəhər: ");
        var city = Console.ReadLine();

        Console.Write("Telefon: ");
        var phone = Console.ReadLine();








        switch (choice)
        {
            case "1":
                if (authService.RegisterWorker(username, password, email,name,surname,city,phone,age,unitOfWork))
                    Console.WriteLine("İşçi olaraq qeydiyyat uğurla tamamlandı!");
                else
                    Console.WriteLine("Bu istifadəçi adı artıq mövcuddur. Yenidən cəhd edin.");
                break;

            case "2":
                if (authService.RegisterEmployer(username, password, email, name, surname, city, phone, age, unitOfWork))
                    Console.WriteLine("İşəgötürən olaraq qeydiyyat uğurla tamamlandı!");
                else
                    Console.WriteLine("Bu istifadəçi adı artıq mövcuddur. Yenidən cəhd edin.");
                break;

            default:
                Console.WriteLine("Yanlış seçim! Qeydiyyat baş tutmadı.");
                break;
        }
    }

    private static void HandleLogin(AuthService authService, WorkerRepository workerRepo, EmployerRepository employerRepo, UnitOfWork unitOfWork)
    {
        Console.WriteLine("\nDaxil ol:");
     
        Console.Write("İstifadəçi adı: ");
        
        
        var username = Console.ReadLine();

        Console.Write("Parol: ");
        var password = Console.ReadLine();

        var user = authService.Authenticate(username, password);
        if (user != null)
        {
            Console.WriteLine($"Xoş gəldiniz, {user.Name}!");

            if (user is Worker worker)
            {
                WorkerActions.WorkerPanel(worker, workerRepo, employerRepo, unitOfWork);
            }
            else if (user is Employer employer)
            {
                EmployerActions.EmployerPanel(employer, employerRepo, workerRepo, unitOfWork);
            }
        }
        else
        {
            Console.WriteLine("İstifadəçi adı və ya parol səhvdir.");
        }
    }
}
