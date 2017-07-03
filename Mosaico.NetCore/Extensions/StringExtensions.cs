using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mosaico.NetCore.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Gets specified number of characters from left of string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Left(this string s, int count)
        {
            if (s == null)
            {
                return null;
            }

            if (s.Length <= count)
            {
                return s;
            }

            return s.Substring(0, count);
        }

        /// <summary>
        /// Returns all characters to the left of the first occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LeftOf(this string s, char value)
        {
            if (s == null)
            {
                return null;
            }

            int index = s.IndexOf(value);
            if (index != -1)
            {
                return s.Substring(0, index);
            }
            return s;
        }

        /// <summary>
        /// Returns all characters to the left of the [n]th occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string LeftOf(this string s, char value, int n)
        {
            if (s == null)
            {
                return null;
            }

            int index = -1;
            while (n > 0)
            {
                index = s.IndexOf(value, index + 1);
                if (index == -1)
                { break; }
                --n;
            }
            if (index != -1)
            {
                return s.Substring(0, index);
            }
            return s;
        }

        /// <summary>
        /// Returns all characters to the left of the first occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LeftOf(this string s, string value)
        {
            if (value == null)
            {
                return null;
            }

            int index = s.IndexOf(value);
            if (index != -1)
            {
                return s.Substring(0, index);
            }
            return s;
        }

        /// <summary>
        /// Returns all characters to the left of the last occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LeftOfLastIndexOf(this string s, char value)
        {
            if (s == null)
            {
                return null;
            }

            string ret = s;
            int index = s.LastIndexOf(value);
            if (index != -1)
            {
                ret = s.Substring(0, index);
            }
            return ret;
        }

        /// <summary>
        /// Returns all characters to the left of the last occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LeftOfLastIndexOf(this string s, string value)
        {
            if (s == null)
            {
                return null;
            }

            string ret = s;
            int index = s.LastIndexOf(value);
            if (index != -1)
            {
                ret = s.Substring(0, index);
            }
            return ret;
        }

        /// <summary>
        /// Gets specified number of characters from right of string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Right(this string s, int count)
        {
            if (s == null)
            {
                return null;
            }

            if (s.Length <= count)
            {
                return s;
            }

            return s.Substring(s.Length - count, count);
        }

        /// <summary>
        /// Returns all characters to the right of the first occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RightOf(this string s, char value)
        {
            if (s == null)
            {
                return null;
            }

            int index = s.IndexOf(value);
            if (index != -1)
            {
                return s.Substring(index + 1);
            }
            return s;
        }

        /// <summary>
        /// Returns all characters to the right of the [n]th occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string RightOf(this string s, char value, int n)
        {
            if (s == null)
            {
                return null;
            }

            int index = -1;
            while (n > 0)
            {
                index = s.IndexOf(value, index + 1);
                if (index == -1)
                { break; }
                --n;
            }

            if (index != -1)
            {
                return s.Substring(index + 1);
            }
            return s;
        }

        /// <summary>
        /// Returns all characters to the right of the first occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RightOf(this string s, string value)
        {
            if (s == null)
            {
                return null;
            }

            int index = s.IndexOf(value);
            if (index != -1)
            {
                return s.Substring(index + 1);
            }
            return s;
        }

        /// <summary>
        /// Returns all characters to the right of the last occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RightOfLastIndexOf(this string s, char value)
        {
            if (s == null)
            {
                return null;
            }

            string ret = string.Empty;
            int index = s.LastIndexOf(value);
            if (index != -1)
            {
                ret = s.Substring(index + 1);
            }
            return ret;
        }

        /// <summary>
        /// Returns all characters to the right of the last occurrence of [value] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RightOfLastIndexOf(this string s, string value)
        {
            if (s == null)
            {
                return null;
            }

            string ret = string.Empty;
            int index = s.LastIndexOf(value);
            if (index != -1)
            {
                ret = s.Substring(index + 1);
            }
            return ret;
        }
    }
}
