using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Validators.User
{
    public class UserForRegistrationDtoValidator : AbstractValidator<UserForRegistrationDto>
    {
        public UserForRegistrationDtoValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .Length(1, 30);
            RuleFor(u => u.LastName)
                .NotEmpty()
                .Length(1, 30);
            RuleFor(u => u.Password)
                .NotEmpty()
                .Length(5, 30)
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
    }
}
