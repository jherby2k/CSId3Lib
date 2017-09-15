// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.

using System;

namespace Id3Lib
{
    /// <summary>
    /// Provides static methods to compare, find, copy and clear a byte array.
    /// </summary>
    internal static class Memory
    {
        #region Methods

        /// <summary>
        /// Compare two byte arrays and determine if they are equal
        /// </summary>
        /// <param name="b1">First byte array</param>
        /// <param name="b2">Second byte array</param>
        /// <returns>Returns true if the arrays are equal</returns>
        public static bool Compare(byte[] b1, byte[] b2)
        {
            if (b1 == null)
                throw new ArgumentNullException("b1");
            if (b2 == null)
                throw new ArgumentNullException("b2");

            if (b1.Length != b2.Length)
            {
                return false;
            }
            for (int n = 0; n < b1.Length; n++)
            {
                if (b1[n] != b2[n])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Create a new array to hold specified bytes copied out of the source array
        /// </summary>
        /// <param name="src">Source array</param>
        /// <param name="srcIndex">Offset of the source array</param>
        /// <returns>Destination array</returns>
        /// <param name="count">Number of bytes to extract</param>
        public static byte[] Extract(byte[] src, int srcIndex, int count)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (src == null || srcIndex < 0 || count < 0)
                throw new InvalidOperationException();

            if (src.Length - srcIndex < count)
                throw new InvalidOperationException();

            byte[] dst = new byte[count];
            Array.Copy(src, srcIndex, dst, 0, count);
            return dst;
        }

        /// <summary>
        /// Find a byte in the array
        /// </summary>
        /// <param name="src">Source array</param>
        /// <param name="val">Byte value to find</param>
        /// <param name="index">Offset of the source array</param>
        /// <returns></returns>
        public static int FindByte(byte[] src, byte val, int index)
        {
            int size = src.Length;

            if (index > size)
                throw new InvalidOperationException();

            for (int n = index; n < size; n++)
            {
                if (src[n] == val)
                {
                    return n - index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find a short in the array
        /// </summary>
        /// <param name="src">Source array</param>
        /// <param name="val">Short value to find</param>
        /// <param name="index">Offset of the source array</param>
        /// <returns></returns>
        public static int FindShort(byte[] src, short val, int index)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            int size = src.Length;
            if (index > size)
                throw new InvalidOperationException();

            for (int n = index; n < size; n += 2)
            {
                if (BitConverter.ToInt16(src, n) == val)
                {
                    return n - index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Clear an array
        /// </summary>
        /// <param name="dst">Source array</param>
        /// <param name="begin">Start position; first byte to clear</param>
        /// <param name="end">End position; first byte not to clear</param>
        public static void Clear(byte[] dst, int begin, int end)
        {
            if (dst == null)
                throw new ArgumentNullException("dst");
            if (begin > end || begin > dst.Length || end > dst.Length)
                throw new InvalidOperationException();

            Array.Clear(dst, begin, end - begin);
        }

        /// <summary>
        /// Get a unsigned long from a byte array
        /// </summary>
        /// <param name="value">a byte array from 1 to 8 bytes</param>
        /// <returns>unsigned long</returns>
        public static ulong ToInt64(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length > 8)
                throw new InvalidOperationException("The count is to large to be stored");

            return BitConverter.ToUInt64(value, 0);
        }

        /// <summary>
        /// get an array from the unsigned long of exactly 8 bytes
        /// </summary>
        /// <param name="value">unsigned long to convert to an array</param>
        /// <returns>the used bytes form the unsigned long</returns>
        public static byte[] GetBytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        #endregion
    }
}
