using FluentValidation;
using ProductStorage.BLL.Models;

namespace ProductStorage.BLL.Validators;

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type is not null);
        RuleFor(x => x.WarehouseNumber).GreaterThan(0).When(x => x.WarehouseNumber is not null);
    }
}
