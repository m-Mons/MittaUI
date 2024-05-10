namespace MittaUI.Runtime.Extension
{
    public static class StringExtensions
    {
        public static string GetSpace(int count)
        {
            var space = string.Empty;
            for (var i = 0; i < count; i++) space += " ";

            return space;
        }
    }
}