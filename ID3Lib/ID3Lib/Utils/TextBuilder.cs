// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Id3Lib.Exceptions;
using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// Type of text used in frame
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    public enum TextCode : byte
    {
        /// <summary>
        /// ASCII(ISO-8859-1)
        /// </summary>
        Ascii = 0x00,
        /// <summary>
        /// Unicode with BOM
        /// </summary>
        Utf16 = 0x01,
        /// <summary>
        /// BigEndian Unicode without BOM
        /// </summary>
        Utf16BE = 0x02,
        /// <summary>
        /// Encoded Unicode
        /// </summary>
        Utf8 = 0x03
    }

    /// <summary>
    /// Manages binary to text and vice versa format conversions.
    /// </summary>
    static class TextBuilder
    {
        [NotNull]
        internal static string ReadText([NotNull] byte[] frame, ref int index, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    return ReadASCII(frame, ref index);
                case TextCode.Utf16:
                    return ReadUTF16(frame, ref index);
                case TextCode.Utf16BE:
                    return ReadUTF16BE(frame, ref index);
                case TextCode.Utf8:
                    return ReadUTF8(frame, ref index);
                default:
                    throw new InvalidFrameException("Invalid text code string type.");
            }
        }

        [Pure, NotNull]
        internal static string ReadTextEnd([NotNull] byte[] frame, int index, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                        return ReadASCIIEnd(frame, index);
                case TextCode.Utf16:
                        return ReadUTF16End(frame, index);
                case TextCode.Utf16BE:
                        return ReadUTF16BEEnd(frame, index);
                case TextCode.Utf8:
                        return ReadUTF8End(frame, index);
                default:
                        throw new InvalidFrameException("Invalid text code string type.");
            }
        }

        [NotNull]
        internal static string ReadASCII([NotNull] byte[] frame, ref int index)
        {
            var text = string.Empty;
            var count = Memory.FindByte(frame, 0, index);
            if (count == -1)
                throw new InvalidFrameException("Invalid ASCII string size");

            if (count > 0)
            {
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1252); // Should be ASCII
                text = encoding.GetString(frame, index, count);
                index += count; // add the read bytes
            }

            index++; // jump an end of line byte
            return text;
        }

        [Pure, NotNull]
        internal static byte[] WriteText([NotNull] string text, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    return WriteASCII(text);
                case TextCode.Utf16:
                    return WriteUTF16(text);
                case TextCode.Utf16BE:
                    return WriteUTF16BE(text);
                case TextCode.Utf8:
                    return WriteUTF8(text);
                default:
                    throw new InvalidFrameException("Invalid text code string type.");
            }
        }

        [Pure, NotNull]
        internal static byte[] WriteTextEnd([NotNull] string text, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    return WriteASCIIEnd(text);
                case TextCode.Utf16:
                    return WriteUTF16End(text);
                case TextCode.Utf16BE:
                    return WriteUTF16BEEnd(text);
                case TextCode.Utf8:
                    return WriteUTF8End(text);
                default:
                    throw new InvalidFrameException("Invalid text code string type.");
            }
        }

        [Pure, NotNull]
        internal static byte[] WriteASCII([NotNull] string text)
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text)) //Write a null string
                {
                    writer.Write((byte) 0);
                    return buffer.ToArray();
                }

                writer.Write(CodePagesEncodingProvider.Instance.GetEncoding(1252).GetBytes(text));
                writer.Write((byte) 0); //EOL
                return buffer.ToArray();
            }
        }

        [NotNull]
        static string ReadUTF16([NotNull] byte[] frame, ref int index)
        {
            // check for empty string first, and throw a useful exception
            // otherwise we'll get an out-of-range exception when we look for the BOM
            if (index >= frame.Length - 2)
                throw new InvalidFrameException("ReadUTF16: string must be terminated");

            if (frame[index] == 0xfe && frame[index + 1] == 0xff) // Big Endian
            {
                index += 2;
                return ReadUTF16BE(frame, ref index);
            }

            if (frame[index] == 0xff && frame[index + 1] == 0xfe) // Little Endian
            {
                index += 2;
                return ReadUTF16LE(frame, ref index);
            }

            if (frame[index] == 0x00 && frame[index + 1] == 0x00) // empty string
            {
                index += 2;
                return string.Empty;
            }

            throw new InvalidFrameException("Invalid UTF16 string.");
        }

        [NotNull]
        static string ReadUTF16BE([NotNull] byte[] frame, ref int index)
        {
            var encoding = new UnicodeEncoding(true, false);
            var count = Memory.FindShort(frame, 0, index);
            if (count == -1)
                throw new InvalidFrameException("Invalid UTF16BE string size");

            // we can safely let count==0 fall through
            var text = encoding.GetString(frame, index, count);
            index += count; // add the bytes read
            index += 2; // skip the EOL
            return text;
        }

        [NotNull]
        static string ReadUTF16LE([NotNull] byte[] frame, ref int index)
        {
            var encoding = new UnicodeEncoding(false, false);
            var count = Memory.FindShort(frame, 0, index);
            if (count == -1)
                throw new InvalidFrameException("Invalid UTF16LE string size");

            // we can safely let count==0 fall through
            var text = encoding.GetString(frame, index, count);
            index += count; // add the bytes read
            index += 2; // skip the EOL
            return text;
        }

        [NotNull]
        static string ReadUTF8([NotNull] byte[] frame, ref int index)
        {
            string text = string.Empty;
            var count = Memory.FindByte(frame, 0, index);
            if (count == -1)
                throw new InvalidFrameException("Invalid UTF8 string size");
            if (count > 0)
            {
                text = Encoding.UTF8.GetString(frame, index, count);
                index += count; // add the read bytes
            }

            index++; // jump an end of line byte
            return text;
        }

        [Pure, NotNull]
        static string ReadASCIIEnd([NotNull] byte[] frame, int index)
        {
            return CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(frame, index, frame.Length - index);
        }

        [Pure, NotNull]
        static string ReadUTF16End([NotNull] byte[] frame, int index)
        {
            // check for empty string first
            // otherwise we'll get an exception when we look for the BOM
            // SourceForge bug ID: 2686976
            if (index >= frame.Length - 2)
                return string.Empty;

            if (frame[index] == 0xfe && frame[index + 1] == 0xff) // Big Endian
                return ReadUTF16BEEnd(frame, index + 2);

            if (frame[index] == 0xff && frame[index + 1] == 0xfe) // Little Endian
                return ReadUTF16LEEnd(frame, index + 2);

            throw new InvalidFrameException("Invalid UTF16 string.");
        }

        [Pure, NotNull]
        static string ReadUTF16BEEnd([NotNull] byte[] frame, int index)
        {
            return new UnicodeEncoding(true, false).GetString(frame, index, frame.Length - index);
        }

        [Pure, NotNull]
        static string ReadUTF16LEEnd([NotNull] byte[] frame, int index)
        {
            return new UnicodeEncoding(false, false).GetString(frame, index, frame.Length - index);
        }

        [Pure, NotNull]
        static string ReadUTF8End([NotNull] byte[] frame, int index)
        {
            return Encoding.UTF8.GetString(frame, index, frame.Length - index);
        }

        [Pure, NotNull]
        static byte[] WriteUTF16([NotNull] string text)
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text)) //Write a null string
                {
                    writer.Write((ushort) 0);
                    return buffer.ToArray();
                }

                writer.Write((byte) 0xff); //Little endian, we have UTF16BE for big endian
                writer.Write((byte) 0xfe);
                writer.Write(new UnicodeEncoding(false, false).GetBytes(text));
                writer.Write((ushort) 0);
                return buffer.ToArray();
            }
        }

        [Pure, NotNull]
        static byte[] WriteUTF16BE([NotNull] string text)
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text)) //Write a null string
                {
                    writer.Write((ushort) 0);
                    return buffer.ToArray();
                }

                writer.Write(new UnicodeEncoding(true, false).GetBytes(text));
                writer.Write((ushort) 0);
                return buffer.ToArray();
            }
        }

        [Pure, NotNull]
        static byte[] WriteUTF8([NotNull] string text)
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text)) //Write a null string
                {
                    writer.Write((byte) 0);
                    return buffer.ToArray();
                }

                writer.Write(Encoding.UTF8.GetBytes(text));
                writer.Write((byte) 0);
                return buffer.ToArray();
            }
        }

        [Pure, NotNull]
        static byte[] WriteASCIIEnd([NotNull] string text)
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text))
                    return buffer.ToArray();

                writer.Write(CodePagesEncodingProvider.Instance.GetEncoding(1252).GetBytes(text));
                return buffer.ToArray();
            }
        }

        [Pure, NotNull]
        static byte[] WriteUTF16End([NotNull] string text)
        {
            using (var buffer = new MemoryStream(text.Length + 2))
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text)) //Write a null string
                    return buffer.ToArray();
                writer.Write((byte) 0xff); // Little endian
                writer.Write((byte) 0xfe);
                writer.Write(new UnicodeEncoding(false, false).GetBytes(text));
                return buffer.ToArray();
            }
        }

        [Pure, NotNull]
        static byte[] WriteUTF16BEEnd([NotNull] string text)
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text)) //Write a null string
                    return buffer.ToArray();
                writer.Write(new UnicodeEncoding(true, false).GetBytes(text));
                return buffer.ToArray();
            }
        }

        [Pure, NotNull]
        static byte[] WriteUTF8End([NotNull] string text)
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                if (string.IsNullOrEmpty(text)) //Write a null string
                {
                    return buffer.ToArray();
                }

                writer.Write(Encoding.UTF8.GetBytes(text));
                return buffer.ToArray();
            }
        }
    }
}
