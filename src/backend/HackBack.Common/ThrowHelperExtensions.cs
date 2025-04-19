namespace HackBack.Common
{
    public static class ThrowHelperExtensions
    {
        public static void ThenThrow(this bool condition, Exception exception)
        {
            if (condition)
                throw exception;
        }
    }
}
