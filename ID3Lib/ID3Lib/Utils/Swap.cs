// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.

using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// Performs byte swapping.
    /// </summary>
    static class Swap
    {
        [Pure]
        internal static int Int32(int val)
        {
            return (int) UInt32((uint) val);
        }

        [Pure]
        internal static uint UInt32(uint val)
        {
            var retval = (val & 0xff) << 24;
            retval |= (val & 0xff00) << 8;
            retval |= (val & 0xff0000) >> 8;
            retval |= (val & 0xff000000) >> 24;
            return retval;
        }

        [Pure]
        internal static ushort UInt16(ushort val)
        {
            var retval = ((uint) val & 0xff) << 8;
            retval |= ((uint) val & 0xff00) >> 8;
            return (ushort) retval;
        }
    }
}
