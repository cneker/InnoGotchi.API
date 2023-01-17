using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.Farm;

namespace InnoGotchi.Application.Validators.Farm
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
