using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.Farm;

namespace InnoGotchi.Application.Validators.Farm
{
    public class FarmForCreationDtoValidator : AbstractValidator<FarmForCreationDto>
    {
        public FarmForCreationDtoValidator()
        {
            RuleFor(f => f.Name)
                .NotEmpty()
                .Length(1, 20);
        }
    }
}
