using FluentValidation;
using InnoGotchi.Application.DataTransferObjects;

namespace InnoGotchi.Application.Validators
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
