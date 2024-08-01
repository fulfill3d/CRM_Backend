using FluentValidation;

namespace CRM.API.Client.Service.Data.Models.Request
{
    public class PaginationParameters
    {
        public int PageSize { get; set; } = 25;

        public int PageAfter { get; set; } = 0;

        public override string ToString()
        {
            return $"pageSize={PageSize}&pageAfter={PageAfter}";
        }
    }

    public class PaginationParametersValidator : AbstractValidator<PaginationParameters>
    {
        public PaginationParametersValidator()
        {
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .OverridePropertyName("page_size")
                .WithMessage("Page size must be between 1 and 100")
                .LessThanOrEqualTo(100)
                .OverridePropertyName("page_size")
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.PageAfter)
                .GreaterThanOrEqualTo(0)
                .WithName("page_after")
                .WithMessage("Page after must be greater then or equal to 0");;
        }
    }
}