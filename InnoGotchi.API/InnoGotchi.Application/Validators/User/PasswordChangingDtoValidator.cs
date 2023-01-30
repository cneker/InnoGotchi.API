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
                .Equal(p => p.ConfirmedPassword)
                .NotEqual(p => p.OldPassword);
            RuleFor(p => p.OldPassword)
                .NotEmpty();
        }
    }
}
