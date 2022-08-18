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

        public static string Join<T>(this T[] array, string joiner)
        {
            string output = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i < array.Length - 1)
                {
                    output += array[i] + joiner;
                }
                else
                {
                    output += array[i];
                }
            }

            return output;
        }
    }
}
