using FluentValidation;

namespace FileStreaming.Api.Application.Common;

// Base para validar qualquer query paginada; cada query herda e adiciona as próprias regras
public abstract class PagedQueryValidator<TQuery> : AbstractValidator<TQuery> where TQuery : PagedQuery
{
    protected PagedQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, PagedQuery.MaxPageSize)
            .WithMessage($"O tamanho da página deve estar entre 1 e {PagedQuery.MaxPageSize}.");
    }
}
