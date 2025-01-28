using Boss.Models;
using Boss.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boss.Utilities;

namespace Boss.Services
{
    public class AuthService
    {
        private readonly Dictionary<string, string> _verificationCodes = new();

        private readonly WorkerRepository _workerRepo;
        private readonly EmployerRepository _employerRepo;

        public AuthService(WorkerRepository workerRepo, EmployerRepository employerRepo)
        {
            _workerRepo = workerRepo;
            _employerRepo = employerRepo;
        }

      






        public bool RegisterWorker(string username, string password, string email,string name,string surname,string city,string phone,int age, UnitOfWork unitOfWork)
        {
            if (_workerRepo.GetAll().Any(w => w.Username == username))
            {
                Console.WriteLine("Bu istifadəçi adı artıq mövcuddur.");
                return false;
            }

            if (IsEmailRegistered(email))
            {
                Console.WriteLine("Bu email artıq qeydiyyatdan keçib.");
                return false;
            }
            if (!PasswordValidator.IsStrongPassword(password))
            {
                Console.WriteLine("Parol güclü deyil! Ən azı bir böyük hərf, bir rəqəm, bir xüsusi simvol daxil etməli və 8 simvoldan uzun olmalıdır.");
                return false;
            }









          
            var verificationCode = VerificationCodeGenerator.GenerateCode();
            _verificationCodes[email] = verificationCode;

         
            EmailService.SendEmail(
                email,
                "Təsdiq Kodu",
                $"Salam {username},<br><br>Sizin təsdiq kodunuz: <strong>{verificationCode}</strong><br>Hörmətlə, Komandamız."
            );

            Console.WriteLine("Email təsdiq kodu göndərildi.");
            Console.Write("Təsdiq kodunu daxil edin: ");
            var inputCode = Console.ReadLine();

            if (inputCode != verificationCode)
            {
                Console.WriteLine("Təsdiq kodu düzgün deyil. Qeydiyyat uğursuz oldu.");
                return false;
            }

            var worker = new Worker
            {
                Id = _workerRepo.GetAll().Count() + 1,
                Username = username,
                Password = password,
                Email= email,
                Name = name,
                Surname = surname,
                City = city,
                Phone = phone,
                Age = age,


            };
            _workerRepo.Add(worker);
           

            Console.WriteLine("Qeydiyyat uğurla tamamlandı!");
            unitOfWork.SaveData();

          
            return true;
        }


        public bool 
            
            
            RegisterEmployer(string username, string password, string email, string name, string surname, string city, string phone, int age, UnitOfWork unitOfWork)
        {
            if (_employerRepo.GetAll().Any(e => e.Name == username))
            {
                Console.WriteLine("Bu istifadəçi adı artıq mövcuddur.");
                return false;
            }

            if (IsEmailRegistered(email))
            {
                Console.WriteLine("Bu email artıq qeydiyyatdan keçib.");
                return false;
            }
            if (!PasswordValidator.IsStrongPassword(password))
            {
                Console.WriteLine("Parol güclü deyil! Ən azı bir böyük hərf, bir rəqəm, bir xüsusi simvol daxil etməli və 8 simvoldan uzun olmalıdır.");
                return false;
            }
          
            var verificationCode = VerificationCodeGenerator.GenerateCode();
            _verificationCodes[email] = verificationCode;

           
            EmailService.SendEmail(
                email,
                "Təsdiq Kodu",
                $"Salam {username},<br><br>Sizin təsdiq kodunuz: <strong>{verificationCode}</strong><br>Hörmətlə, Komandamız."
            );

            Console.WriteLine("Email təsdiq kodu göndərildi.");
            Console.Write("Təsdiq kodunu daxil edin: ");
            var inputCode = Console.ReadLine();

            if (inputCode != verificationCode)
            {
                Console.WriteLine("Təsdiq kodu düzgün deyil. Qeydiyyat uğursuz oldu.");
                return false;
            }

            var employer = new Employer
            {
                Id = _workerRepo.GetAll().Count() + 1,
                Username = username,
                Password = password,
                Email = email,
                Name = name,
                Surname = surname,
                City = city,
                Phone = phone,
                Age = age,
            };
            _employerRepo.Add(employer);
            


            Console.WriteLine("Qeydiyyat uğurla tamamlandı!");
            unitOfWork.SaveData();

            return true;
        }


        public User Authenticate(string username, string password)
        {
            try
            {
                var worker = _workerRepo.GetAll().FirstOrDefault(w => w.Username == username && w.Password == password);
                if (worker != null) return worker;

                var employer = _employerRepo.GetAll().FirstOrDefault(e => e.Username == username && e.Password == password);
                if (employer != null) return employer;

                throw new Exception("Daxil edilmiş məlumatlara uyğun istifadəçi tapılmadı.");
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "Authenticate");
                return null;
            }
        }

        public bool IsUsernameTaken(string username)
        {
            return _workerRepo.GetAll().Any(w => w.Username == username) ||
                   _employerRepo.GetAll().Any(e => e.Username == username);
        }

        public bool IsEmailRegistered(string email)
        {
            return _workerRepo.GetAll().Any(w => w.Email == email) ||
                   _employerRepo.GetAll().Any(e => e.Email == email);
        }
        public bool IsNameSurnameTaken(string name, string surname)
        {
            return _workerRepo.GetAll().Any(w => w.Name == name && w.Surname == surname) ||
                   _employerRepo.GetAll().Any(e => e.Name == name && e.Surname == surname);
        }



    }


}
