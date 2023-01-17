using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Validators.User
{
    public class UserForUpdateDtoValidator : AbstractValidator<UserForUpdateDto>
    {
        public UserForUpdateDtoValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .Length(1, 30);
            RuleFor(u => u.LastName)
                .NotEmpty()
                .Length(1, 30);
            RuleFor(u => u.NewAvatar)
                .NotEmpty();
        }
    }
}
