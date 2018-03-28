// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.

using System;
using System.Buffers.Binary;
using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// Provides static methods to compare, find, copy and clear a byte array.
    /// </summary>
    static class Memory
    {
        /// <summary>
        /// Compare two byte arrays and determine if they are equal
        /// </summary>
        /// <param name="b1">First byte array</param>
        /// <param name="b2">Second byte array</param>
        /// <returns>Returns true if the arrays are equal</returns>
        [Pure]
        internal static bool Compare(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
        {
            return b1.SequenceCompareTo(b2) == 0;
        }

        /// <summary>
        /// Create a new array to hold specified bytes copied out of the source array
        /// </summary>
        /// <param name="src">Source array</param>
        /// <param name="srcIndex">Offset of the source array</param>
        /// <returns>Destination array</returns>
        /// <param name="count">Number of bytes to extract</param>
        [Pure, NotNull]
        internal static byte[] Extract(ReadOnlySpan<byte> src, int srcIndex, int count)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (src == null || srcIndex < 0 || count < 0)
                throw new InvalidOperationException();

            if (src.Length - srcIndex < count)
                throw new InvalidOperationException();

            return src.Slice(srcIndex, count).ToArray();
        }

        /// <summary>
        /// Find a byte in the array
        /// </summary>
        /// <param name="src">Source array</param>
        /// <param name="val">Byte value to find</param>
        /// <param name="index">Offset of the source array</param>
        /// <returns></returns>
        [Pure]
        internal static int FindByte(ReadOnlySpan<byte> src, byte val, int index)
        {
            if (index > src.Length)
                throw new InvalidOperationException();

            return src.Slice(index).IndexOf(val);
        }

        /// <summary>
        /// Find a short in the array
        /// </summary>
        /// <param name="src">Source array</param>
        /// <param name="val">Short value to find</param>
        /// <param name="index">Offset of the source array</param>
        /// <returns></returns>
        [Pure]
        internal static int FindShort(ReadOnlySpan<byte> src, short val, int index)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            int size = src.Length;
            if (index > size)
                throw new InvalidOperationException();

            for (var n = index; n < size; n += 2)
                if (BinaryPrimitives.ReadInt16LittleEndian(src.Slice(n)) == val)
                    return n - index;
            return -1;
        }

        /// <summary>
        /// Clear an array
        /// </summary>
        /// <param name="dst">Source array</param>
        /// <param name="begin">Start position; first byte to clear</param>
        /// <param name="end">End position; first byte not to clear</param>
        internal static void Clear(Span<byte> dst, int begin, int end)
        {
            if (dst == null)
                throw new ArgumentNullException("dst");
            if (begin > end || begin > dst.Length || end > dst.Length)
                throw new InvalidOperationException();

            dst.Slice(begin, end - begin).Clear();
        }

        /// <summary>
        /// Get a unsigned long from a byte array
        /// </summary>
        /// <param name="value">a byte array from 1 to 8 bytes</param>
        /// <returns>unsigned long</returns>
        [Pure]
        internal static ulong ToInt64(ReadOnlySpan<byte> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length > 8)
                throw new InvalidOperationException("The count is to large to be stored");

            return BinaryPrimitives.ReadUInt64LittleEndian(value);
        }

        /// <summary>
        /// get an array from the unsigned long of exactly 8 bytes
        /// </summary>
        /// <param name="value">unsigned long to convert to an array</param>
        /// <returns>the used bytes form the unsigned long</returns>
        [Pure, NotNull]
        internal static byte[] GetBytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
