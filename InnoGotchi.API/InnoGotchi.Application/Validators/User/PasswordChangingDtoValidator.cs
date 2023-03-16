using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Validators.User
{
    public class PasswordChangingDtoValidator : AbstractValidator<PasswordChangingDto>
    {
        public PasswordChangingDtoValidator()
        {
            RuleFor(p => p.NewPassword)
                .NotEmpty()
                .Length(5, 30)
                .Equal(p => p.ConfirmedPassword)
                .NotEqual(p => p.OldPassword);
            RuleFor(p => p.ConfirmedPassword)
                .NotEmpty()
                .Length(5, 30);
            RuleFor(p => p.OldPassword)
                .NotEmpty()
                .Length(5, 30);
        }
    }
}
