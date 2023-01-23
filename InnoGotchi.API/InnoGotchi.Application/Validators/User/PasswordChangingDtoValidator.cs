using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Validators.User
{
    public class PasswordChangingDtoValidator : AbstractValidator<PasswordChangingDto>
    {
        public PasswordChangingDtoValidator()
        {
            RuleFor(p => p.NewPassword)
                .NotEmpty();
            RuleFor(p => p.OldPassword)
                .NotEmpty();
        }
    }
}
