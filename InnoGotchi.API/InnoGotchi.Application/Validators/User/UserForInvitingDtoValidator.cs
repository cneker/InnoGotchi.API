using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Validators.User
{
    public class UserForInvitingDtoValidator : AbstractValidator<UserForInvitingDto>
    {
        public UserForInvitingDtoValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
