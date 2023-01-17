using FluentValidation;
using InnoGotchi.Application.DataTransferObjects;

namespace InnoGotchi.Application.Validators
{
    public class FarmForUpdateDtoValidator : AbstractValidator<FarmForUpdateDto>
    {
        public FarmForUpdateDtoValidator()
        {
            RuleFor(f => f.Name)
                .NotEmpty()
                .Length(1, 20);
        }
    }
}
