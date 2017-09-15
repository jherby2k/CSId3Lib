// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using Id3Lib.Exceptions;

namespace Id3Lib
{
    /// <summary>
    /// Provides static methods for making ID3v2 un-synchronisation
    /// </summary>
    /// <remarks>
    /// This helper class takes care of the synchronisation and un-synchronisation needs.
    /// The purpose of un-synchronisation is to make the ID3v2 tag as compatible as possible
    /// with existing software and hardware.
    /// 
    /// Synch safe integers are integers that keep its highest byte bit (bit 7) zeroed, making seven bits
    /// out of every eight available.
    /// </remarks>
    internal static class Sync
    {
        #region Methods
        /// <summary>
        /// Converts from a sync-safe integer to a normal integer
        /// </summary>
        /// <param name="val">Little-endian Sync-safe value</param>
        /// <returns>Little-endian normal value</returns>
        public static uint Unsafe(uint val)
        {
            byte[] value = BitConverter.GetBytes(val);
            if (value[0] > 0x7f || value[1] > 0x7f || value[2] > 0x7f || value[3] > 0x7f)
                throw new InvalidTagException("Sync-safe value corrupted");

            byte[] sync = new byte[4];
            sync[0] = (byte)(((value[0] >> 0) & 0x7f) | ((value[1] & 0x01) << 7));
            sync[1] = (byte)(((value[1] >> 1) & 0x3f) | ((value[2] & 0x03) << 6));
            sync[2] = (byte)(((value[2] >> 2) & 0x1f) | ((value[3] & 0x07) << 5));
            sync[3] = (byte)(((value[3] >> 3) & 0x0f));
            return BitConverter.ToUInt32(sync, 0);
        }

        /// <summary>
        /// Converts from a normal integer to a sync-safe integer
        /// </summary>
        /// <param name="val">Big-endian normal value</param>
        /// <returns>Big-endian sync-safe value</returns>
        public static uint Safe(uint val)
        {
            if (val > 0x10000000)
                throw new OverflowException("value is too large for a sync-safe integer");

            byte[] value = BitConverter.GetBytes(val);
            byte[] sync = new byte[4];
            sync[0] = (byte)((value[0] >> 0) & 0x7f);
            sync[1] = (byte)(((value[0] >> 7) & 0x01) | (value[1] << 1) & 0x7f);
            sync[2] = (byte)(((value[1] >> 6) & 0x03) | (value[2] << 2) & 0x7f);
            sync[3] = (byte)(((value[2] >> 5) & 0x07) | (value[3] << 3) & 0x7f);
            return BitConverter.ToUInt32(sync, 0);
        }

        /// <summary>
        /// Converts from a sync-safe integer to a normal integer
        /// </summary>
        /// <param name="val">Big-endian Sync-safe value</param>
        /// <returns>Big-endian normal value</returns>
        public static uint UnsafeBigEndian(uint val)
        {
            byte[] value = BitConverter.GetBytes(val);
            if (value[0] > 0x7f || value[1] > 0x7f || value[2] > 0x7f || value[3] > 0x7f)
                throw new InvalidTagException("Sync-safe value corrupted");

            byte[] sync = new byte[4];
            sync[3] = (byte)(((value[3] >> 0) & 0x7f) | ((value[2] & 0x01) << 7));
            sync[2] = (byte)(((value[2] >> 1) & 0x3f) | ((value[1] & 0x03) << 6));
            sync[1] = (byte)(((value[1] >> 2) & 0x1f) | ((value[0] & 0x07) << 5));
            sync[0] = (byte)(((value[0] >> 3) & 0x0f));
            return BitConverter.ToUInt32(sync, 0);
        }

        /// <summary>
        /// Converts from a sync-safe integer to a normal integer
        /// </summary>
        /// <param name="val">Big-endian normal value</param>
        /// <returns>Big-endian sync--safe value</returns>
        public static uint SafeBigEndian(uint val)
        {
            if (val > 0x10000000)
                throw new OverflowException("value is too large for a sync-safe integer");

            byte[] value = BitConverter.GetBytes(val);
            byte[] sync = new byte[4];
            sync[3] = (byte)((value[3] >> 0) & 0x7f);
            sync[2] = (byte)(((value[3] >> 7) & 0x01) | (value[2] << 1) & 0x7f);
            sync[1] = (byte)(((value[2] >> 6) & 0x03) | (value[1] << 2) & 0x7f);
            sync[0] = (byte)(((value[1] >> 5) & 0x07) | (value[0] << 3) & 0x7f);
            return BitConverter.ToUInt32(sync, 0);
        }

        /// <summary>
        /// Convert a sync-safe stream to a normal stream
        /// </summary>
        /// <param name="src">Source stream</param>
        /// <param name="dst">Destination stream</param>
        /// <param name="size">Bytes to be processed</param>
        /// <returns>Number of bytes removed from the original stream</returns>
        public static uint Unsafe(Stream src, Stream dst, uint size)
        {
            var writer = new BinaryWriter(dst);
            var reader = new BinaryReader(src);

            byte last = 0;
            uint syncs = 0, count = 0;

            while (count < size)
            {
                byte val = reader.ReadByte();
                if (last == 0xFF && val == 0x00)
                {
                    syncs++; // skip the sync byte
                }
                else
                {
                    writer.Write(val);
                }
                last = val;
                count++;
            }
            if (last == 0xFF)
            {
                writer.Write((byte)0x00);
                syncs++;
            }
            dst.Seek(0, SeekOrigin.Begin);
            return syncs; //bytes removed from stream
        }

        /// <summary>
        /// Convert from an unsafe or normal stream to a sync-safe stream 
        /// </summary>
        /// <param name="src">Source stream</param>
        /// <param name="dst">Destination stream</param>
        /// <param name="count">Bytes to be processed</param>
        /// <returns>Number of bytes added to the original stream</returns>
        public static uint Safe(Stream src, Stream dst, uint count)
        {
            var writer = new BinaryWriter(dst);
            var reader = new BinaryReader(src);

            byte last = 0;
            uint syncs = 0;

            while (count > 0)
            {
                byte val = reader.ReadByte();
                if (last == 0xFF && (val == 0x00 || val >= 0xE0))
                {
                    writer.Write((byte)0x00);
                    syncs++;
                }
                last = val;
                writer.Write(val);
                count--;
            }
            if (last == 0xFF)
            {
                writer.Write((byte)0x00);
                syncs++;
            }
            return syncs; // bytes added to the stream
        }
        #endregion
    }
}
