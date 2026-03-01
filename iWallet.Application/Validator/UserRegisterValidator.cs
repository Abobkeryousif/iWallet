
using FluentValidation;

namespace iWallet.Application.Validator
{
    public class UserRegisterValidator : AbstractValidator<UserDto>
    {
        public UserRegisterValidator()
        {
            RuleFor(fn=> fn.FirstName)
             .NotEmpty().WithMessage("First Name Is Requierd")
             .MinimumLength(2).WithMessage("First Name Must be long then 2 characters")
             .MaximumLength(10).WithMessage("First Name Must be less then 10 characters");

            RuleFor(ln => ln.LastName)
             .NotEmpty().WithMessage("Last Name Is Requierd")
             .MinimumLength(2).WithMessage("Last Name Must be long then 2 characters")
             .MaximumLength(10).WithMessage("Last Name Must be less then 10 characters");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Email Is Requierd")
                .EmailAddress().WithMessage("Invalid Email Format")
                .MaximumLength(30).WithMessage("Email must be less then 30 characters");

            RuleFor(c=> c.City)
                .MinimumLength(2).WithMessage("Must be long then 2 characters")
                .MaximumLength(31).WithMessage("Max Charchters Is 30");

            RuleFor(p => p.PhoneNumber)
                .NotEmpty().WithMessage("Phone Number Is Requierd")
                .MinimumLength(10).WithMessage("Phone Number Must be more then 10 number")
                .MaximumLength(15).WithMessage("Phone Number Must be less then 15 number");

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("Password Is Requierd")
                .MinimumLength(8).WithMessage("Password Must be more then 8 characters")
                .MaximumLength(15).WithMessage("Password must be less then 15 characters")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one Uppercase letter")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one Lowercase letter")
                .Matches(@"[0-9]+").WithMessage("Password must contain at least one number");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required")
                .InclusiveBetween(new DateTime(1960, 1, 1), new DateTime(2008, 12, 31)).WithMessage("Your Age Must Be legal");
        }
    }
}
