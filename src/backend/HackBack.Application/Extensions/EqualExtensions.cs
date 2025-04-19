namespace HackBack.Application.Extensions
{
    public static class EqualExtensions
    {
        // экстеншен для сравнения двух коллекций
        public static bool IsEqualTo<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            if (source is null && target is null)
            {
                return true;
            }
            if (source is null || target is null)
            {
                return false;
            }

            // Сравниваем коллекции без учета регистра и порядка
            return source
                .Select(item => item?.ToString()?.ToLowerInvariant())
                .OrderBy(item => item)
                .SequenceEqual(
                    target
                    .Select(item => item?.ToString()?.ToLowerInvariant())
                    .OrderBy(item => item)
                );
        }
    }
}
