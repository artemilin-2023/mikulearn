namespace HackBack.Application.Models.Pagination
{
    public record PaginationParams
    {
        /// <summary>
        /// Максимальное допустимое колчество элементов на одной странице.
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// Значение, относительно которого идет отсчет страниц.
        /// </summary>
        public const int StartPage = 0;

        /// <summary>
        /// Номер извлекаемой страницы.
        /// </summary>
        public int PageNumber { get; init; } = StartPage;

        /// <summary>
        /// Максимальное количество элементов на одной странице.
        /// </summary>
        public int PageSize { get; init; } = MaxPageSize;
    }

}
