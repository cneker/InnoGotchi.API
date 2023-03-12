using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.User;

namespace InnoGotchi.Application.Validators.User
{
    public class AvatarChangingDtoValidator : AbstractValidator<AvatarChangingDto>
    {
        public AvatarChangingDtoValidator()
        {
            RuleFor(a => a.FileName)
                .NotEmpty()
                .Matches("\\w\\.[A-Za-z]{3,}");
            RuleFor(a => a.Base64Image)
                .NotEmpty();
        }
    }
}
