namespace FileStreaming.Api.Application.Common;

public abstract record PagedQuery
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    private readonly int _page = 1;
    private readonly int _pageSize = DefaultPageSize;

    // Valores fora do limite são corrigidos na atribuição, para nenhuma consulta receber página inválida
    public int Page
    {
        get => _page;
        init => _page = Math.Max(value, 1);
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}
