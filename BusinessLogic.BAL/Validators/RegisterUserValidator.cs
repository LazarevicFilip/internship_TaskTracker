using DataAccess.DAL;
using Domain.Dto.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators
{
    public class RegisterUserValidator : AbstractValidator<UserRegisterDto>
    {
        private TaskContext _context;
        public RegisterUserValidator(TaskContext context)
        {
            _context = context;
            var nameRegex = "^[A-Z][a-z]{2,}(\\s[A-Z][a-z]{2,})*$";
            RuleFor(x => x.UserName).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username length must be at least 3 characters")
                .MaximumLength(20).WithMessage("Username length must be under than 20 characters")
                .Matches("^(?=[a-zA-Z0-9._]{3,20}$)(?!.*[_.]{2})[^_.].*[^_.]$").WithMessage("Username can only contain letters,digits and _ (underscore)")
                .Must(x => !_context.Users.Any(y => y.UserName == x)).WithMessage("Username {PropertyValue} has aleready been use.");

            RuleFor(x => x.FirstName).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Frist name is requred")
                .MaximumLength(50).WithMessage("Frist name must be under than 50 characters.")
                .Matches(nameRegex).WithMessage("Frist name must be at least 3 characters and must start with a capital letter");

            RuleFor(x => x.LastName).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must be under than 50 characters.")
                .Matches(nameRegex).WithMessage("Last name must be at least 3 characters and must start with a capital letter.");

            RuleFor(x => x.Email).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("{PropertyValue} isn't valid email adress").Must(x => !_context.Users.Any(y => y.Email == x)).WithMessage("Email {PropertyValue} has already been used");

            RuleFor(x => x.Password).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Password is required").Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$").WithMessage("Password must contain at least one lowercase letter,one uppercase letter,digit and special character.");

        }
    }
}
