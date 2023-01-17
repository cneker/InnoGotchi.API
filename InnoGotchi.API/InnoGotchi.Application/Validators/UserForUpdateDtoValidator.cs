using FluentValidation;
using InnoGotchi.Application.DataTransferObjects;

namespace InnoGotchi.Application.Validators
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
