namespace HackBack.Application.Models.Pagination
{
    public record SortParams
    {
        /// <summary>
        /// Название поля, по которому выполняется сортировка.
        /// </summary>
        public required string SortBy { get; init; }

        /// <summary>
        /// Флаг, указывающий направление сортировки: true — по возрастанию, false — по убыванию.
        /// </summary>
        public bool IsAscending { get; init; } = true;
    }
}
