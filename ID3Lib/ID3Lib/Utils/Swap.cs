// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;

namespace Id3Lib
{
    /// <summary>
    /// Performs byte swapping.
    /// </summary>
    internal static class Swap
    {
        #region Methods

        public static int Int32(int val)
        {
            return (int)UInt32((uint)val);
        }

        public static uint UInt32(uint val)
        {
            uint retval = (val & 0xff) << 24;
            retval |= (val & 0xff00) << 8;
            retval |= (val & 0xff0000) >> 8;
            retval |= (val & 0xff000000) >> 24;
            return retval;
        }

        public static short Int16(short val)
        {
            return (short)UInt16((ushort)val);
        }

        public static ushort UInt16(ushort val)
        {
            uint retval = ((uint)val & 0xff) << 8;
            retval |= ((uint)val & 0xff00) >> 8;
            return (ushort)retval;
        }
        #endregion
    }
}
