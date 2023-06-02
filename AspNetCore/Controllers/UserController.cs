using AspNetCore.Data;
using AspNetCore.Models.User;
using AspNetCore.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace AspNetCore.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;

        public UserController(
            DataContext dataContext,
            IHashService hashService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ViewResult SignUp(SignUpFormModel? fromModel)
        {
            SignUpViewModel viewModel = new();
            if (Request.Method == "POST") 
            {
                // Передача формы
                viewModel = ValidateSignUpForm(fromModel);
                viewModel.FormModel = fromModel;
            }
            else
            {
                viewModel.FormModel = null;
            }
            return View(viewModel);
        }

        private SignUpViewModel ValidateSignUpForm (SignUpFormModel formModel)
        {
            SignUpViewModel viewModel = new();

            // Логин
            if (String.IsNullOrEmpty(formModel.Login))
                viewModel.LoginMessage = "Логин не может быть пустым";
            else if (formModel.Login.Length < 2)
                viewModel.LoginMessage = "Логин должен быть не меньше 2 букв";
            else if (_dataContext.Users.Any(u => u.Login == formModel.Login))
                viewModel.LoginMessage = "Данный Логин уже занят";
            else
                viewModel.LoginMessage = null;

            // Пароль
            if (String.IsNullOrEmpty(formModel.Password))
                viewModel.PasswordMessage = "Логин не может быть пустым";
            else if (formModel.Password.Length < 6)
                viewModel.PasswordMessage = "Логин должен быть не меньше 6 символов";
            else if (! Regex.IsMatch(formModel.Password, @"\d"))
                viewModel.PasswordMessage = "Пароль должен содержать цифры";
            else
                viewModel.PasswordMessage = null;

            // Повтор Пароль
            if (formModel.Password != formModel.RepeatPassword)
                viewModel.RepeatMessage = "Пароли не совпадают";
            else if (viewModel.PasswordMessage != null)
                viewModel.RepeatMessage = "Пароль создан неверно";
            else 
                viewModel.RepeatMessage = null;


            // Имя
            if (String.IsNullOrEmpty(formModel.RealName))
                viewModel.RealNameMessage = "Имя не может быть пустым";
            else if (formModel.RealName.Length < 2)
                viewModel.RealNameMessage = "Имя должен быть не меньше 2 букв";
            else
                viewModel.RealNameMessage = null;


            // Подтверждение
            if (!formModel.confirm)
                viewModel.ConfirmMessage = "Подтвердите";
            else
                viewModel.ConfirmMessage = null;


            // Сохранение файла
            if (formModel.Avatar != null) // Файл передан
            {
                if (formModel.Avatar.Length > 1048576)
                {
                    viewModel.AvatarMessage = "Файл слишком большой (Макс 1МБ)";
                }

                if (viewModel.LoginMessage == null &&
                    viewModel.PasswordMessage == null &&
                    viewModel.RepeatMessage == null &&
                    viewModel.EmailMessage == null &&
                    viewModel.ConfirmMessage == null &&
                    viewModel.RealNameMessage == null)
                {
                    String ext = Path.GetExtension(formModel.Avatar.FileName);
                    String name = Guid.NewGuid().ToString() + ext;

                    formModel.Avatar.CopyTo(
                        new FileStream("wwwroot/avatar/" + name, FileMode.Create)
                    );

                    // Все проверки пройдены успешно - добавляем пользователя в БД
                    _dataContext.Users.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Login = formModel.Login,
                        PasswordHash = _hashService.GetHash(formModel.Password),
                        Email = formModel.Email!,
                        CreatedDt = DateTime.Now,
                        Name = formModel.RealName!,
                        Avatar = name
                    });
                    _dataContext.SaveChanges();
                }
                else
                {
                    viewModel.AvatarMessage = "Не все поля регистрации верны";
                }
            }

            
            return viewModel;
        }
    }
}
