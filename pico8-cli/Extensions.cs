using System;

namespace pico8_cli
{
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int end)
        {
            T[] result = new T[end - offset];
            Array.Copy(array, offset, result, 0, end - offset);
            return result;
        }

        public static bool Contains<T>(this T[] array, T value)
        {
            foreach (T entry in array)
            {
                if (entry.Equals(value)) return true;
            }

            return false;
        }
    }
}
