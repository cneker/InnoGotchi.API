using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Validators.User
{
    public class UserInfoForUpdateDtoValidator : AbstractValidator<UserInfoForUpdateDto>
    {
        public UserInfoForUpdateDtoValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .Length(1, 30);
            RuleFor(u => u.LastName)
                .NotEmpty()
                .Length(1, 30);
        }
    }
}
