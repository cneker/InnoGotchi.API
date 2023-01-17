using FluentValidation;
using InnoGotchi.Application.DataTransferObjects;

namespace InnoGotchi.Application.Validators
{
    public class PetForUpdateDtoValidator : AbstractValidator<PetForUpdateDto>
    {
        public PetForUpdateDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .Length(1, 20);
        }
    }
}
