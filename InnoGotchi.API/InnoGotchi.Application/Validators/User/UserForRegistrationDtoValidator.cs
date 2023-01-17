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
        }
    }
}
