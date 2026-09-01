namespace GamedayTracker.Utility
{
    public static class ListExtensions
    {
        private static Random rng = new Random();

        #region SHUFFLE LIST

        public static List<T> Shuffle<T>(this List<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }
        #endregion
    }
}
