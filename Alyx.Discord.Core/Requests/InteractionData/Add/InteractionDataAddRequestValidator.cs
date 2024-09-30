using FluentValidation;

namespace Alyx.Discord.Core.Requests.InteractionData.Add;

internal class InteractionDataAddRequestValidator<T> : AbstractValidator<InteractionDataAddRequest<T>>
{
    public InteractionDataAddRequestValidator()
    {
        RuleFor(x => x.Value)
            .NotNull()
            .WithMessage("Value cannot be null.");
    }
}