// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Id3Lib.Exceptions;
using Id3Lib.Frames;
using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// Manage the Parsing or Creation of binary frames.
    /// </summary>
    /// <remarks>
    /// The <b>FrameHelper</b> is a helper class that receives binary frame from a ID3v1 tag
    /// and returns the correct parsed frame or form a frame creates a binary frame that can be
    /// saved on an ID3v2 tag in a mp3 file.
    /// </remarks>
    sealed class FrameHelper
    {
        readonly byte _version;

        /// <summary>
        /// Create Frames depending on type
        /// </summary>
        /// <param name="header">ID3 Header</param>
        internal FrameHelper([NotNull] TagHeader header)
        {
            _version = header.Version;
        }

        /// <summary>
        /// Create a frame depending on the tag form its binary representation.
        /// </summary>
        /// <param name="frameId">type of frame</param>
        /// <param name="flags">frame flags</param>
        /// <param name="buffer">buffer containing the frame</param>
        /// <returns>Frame of tag type</returns>
        [NotNull]
        internal FrameBase Build([NotNull] string frameId, ushort flags, [NotNull] byte[] buffer)
        {
            // Build a frame
            var frame = FrameFactory.Build(frameId);
            SetFlags(frame, flags);

            var index = 0;
            var size = (uint) buffer.Length;
            Stream stream = new MemoryStream(buffer, false);
            var streamsToClose = new List<Stream>(3) { stream };
            try
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    if (GetGrouping(flags))
                    {
                        frame.Group = reader.ReadByte();
                        index++;
                    }

                    if (frame.Compression)
                    {
                        switch (_version)
                        {
                            case 3:
                                size = Swap.UInt32(reader.ReadUInt32());
                                break;
                            case 4:
                                size = Swap.UInt32(Sync.UnsafeBigEndian(reader.ReadUInt32()));
                                break;
                            default:
                                throw new NotImplementedException($"ID3v2 Version {_version} is not supported.");
                        }
                        index = 0;
                        stream = new InflaterInputStream(stream);
                        streamsToClose.Add(stream);
                    }

                    if (frame.Encryption)
                    {
                        throw new NotImplementedException(
                            "Encryption is not implemented, consequently it is not supported.");
                    }

                    if (frame.Unsynchronisation)
                    {
                        var memoryStream = new MemoryStream();
                        streamsToClose.Add(memoryStream);
                        size = Sync.Unsafe(stream, memoryStream, size);
                        index = 0;
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        stream = memoryStream;
                    }

                    var frameBuffer = new byte[size - index];
                    stream.Read(frameBuffer, 0, (int) (size - index));
                    frame.Parse(frameBuffer);
                    return frame;
                }
            }
            finally
            {
                foreach (var streamToClose in streamsToClose)
                    streamToClose.Close();
            }
        }

        /// <summary>
        /// Build a binary data frame form the frame object.
        /// </summary>
        /// <param name="frame">ID3 Frame</param>
        /// <param name="flags">frame flags</param>
        /// <returns>binary frame representation</returns>
        [NotNull]
        internal byte[] Make([NotNull] FrameBase frame, out ushort flags)
        {
            flags = GetFlags(frame);
            var buffer = frame.Make();

            var memoryStream = new MemoryStream();
            var streamsToClose = new List<Stream>(2) { memoryStream };
            try
            {
                using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    if (frame.Group.HasValue)
                        writer.Write((byte) frame.Group);

                    if (frame.Compression)
                    {
                        switch (_version)
                        {
                            case 3:
                                writer.Write(Swap.Int32(buffer.Length));
                                break;
                            case 4:
                                writer.Write(Sync.UnsafeBigEndian(Swap.UInt32((uint) buffer.Length)));
                                break;
                            default:
                                throw new NotImplementedException($"ID3v2 Version {_version} is not supported.");
                        }

                        var buf = new byte[2048];
                        var deflater = new Deflater(Deflater.BEST_COMPRESSION);
                        deflater.SetInput(buffer, 0, buffer.Length);
                        deflater.Finish();

                        while (!deflater.IsNeedingInput)
                        {
                            var len = deflater.Deflate(buf, 0, buf.Length);
                            if (len <= 0) break;
                            memoryStream.Write(buf, 0, len);
                        }

                        //TODO: Skip and remove invalid frames.
                        if (!deflater.IsNeedingInput)
                            throw new InvalidFrameException($"Can't decompress frame '{frame.FrameId}' missing data");
                    }
                    else
                        memoryStream.Write(buffer, 0, buffer.Length);

                    //TODO: Encryption
                    if (frame.Encryption)
                        throw new NotImplementedException(
                            "Encryption is not implemented, consequently it is not supported.");

                    if (frame.Unsynchronisation)
                    {
                        var synchStream = new MemoryStream();
                        streamsToClose.Add(synchStream);
                        Sync.Unsafe(memoryStream, synchStream, (uint) memoryStream.Position);
                        memoryStream = synchStream;
                    }

                    return memoryStream.ToArray();
                }
            }
            finally
            {
                foreach (var streamToClose in streamsToClose)
                    streamToClose.Close();
            }
        }

        void SetFlags([NotNull] FrameBase frame, ushort flags)
        {
            frame.TagAlter = GetTagAlter(flags);
            frame.FileAlter = GetFileAlter(flags);
            frame.ReadOnly = GetReadOnly(flags);
            frame.Compression = GetCompression(flags);
            frame.Encryption = GetEncryption(flags);
            frame.Unsynchronisation = GetUnsynchronisation(flags);
            frame.DataLength = GetDataLength(flags);
        }

        [Pure]
        bool GetTagAlter(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x8000) > 0;
                case 4:
                    return (flags & 0x4000) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        [Pure]
        bool GetFileAlter(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x4000) > 0;
                case 4:
                    return (flags & 0x2000) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        [Pure]
        bool GetReadOnly(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x2000) > 0;
                case 4:
                    return (flags & 0x1000) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        [Pure]
        bool GetGrouping(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x0020) > 0;
                case 4:
                    return (flags & 0x0040) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        [Pure]
        bool GetCompression(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x0080) > 0;
                case 4:
                    return (flags & 0x0008) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        [Pure]
        bool GetEncryption(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x0040) > 0;
                case 4:
                    return (flags & 0x0004) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        [Pure]
        bool GetUnsynchronisation(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return false;
                case 4:
                    return (flags & 0x0002) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        [Pure]
        bool GetDataLength(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return false;
                case 4:
                    return (flags & 0x0001) > 0;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        ushort GetFlags([NotNull] FrameBase frame)
        {
            ushort flags = 0;
            SetTagAlter(frame.TagAlter, ref flags);
            SetFileAlter(frame.FileAlter, ref flags);
            SetReadOnly(frame.ReadOnly, ref flags);
            SetGrouping(frame.Group.HasValue, ref flags);
            SetCompression(frame.Compression, ref flags);
            SetEncryption(frame.Encryption, ref flags);
            SetUnsynchronisation(frame.Unsynchronisation, ref flags);
            SetDataLength(frame.DataLength, ref flags);
            return flags;
        }

        void SetTagAlter(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    flags = value ? (ushort) (flags | 0x8000) : (ushort) (flags & unchecked((ushort) ~0x8000));
                    break;
                case 4:
                    flags = value ? (ushort) (flags | 0x4000) : (ushort) (flags & unchecked((ushort) ~0x4000));
                    break;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        void SetFileAlter(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    flags = value ? (ushort) (flags | 0x4000) : (ushort) (flags & unchecked((ushort) ~0x4000));
                    break;
                case 4:
                    flags = value ? (ushort) (flags | 0x2000) : (ushort) (flags & unchecked((ushort) ~0x2000));
                    break;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        void SetReadOnly(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                {
                    flags = value ? (ushort) (flags | 0x2000) : (ushort) (flags & unchecked((ushort) ~0x2000));
                    break;
                }
                case 4:
                {
                    flags = value ? (ushort) (flags | 0x1000) : (ushort) (flags & unchecked((ushort) ~0x1000));
                    break;
                }
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        void SetGrouping(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    flags = value ? (ushort) (flags | 0x0020) : (ushort) (flags & unchecked((ushort) ~0x0020));
                    break;
                case 4:
                    flags = value ? (ushort) (flags | 0x0040) : (ushort) (flags & unchecked((ushort) ~0x0040));
                    break;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        void SetCompression(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    flags = value ? (ushort) (flags | 0x0080) : (ushort) (flags & unchecked((ushort) ~0x0080));
                    break;
                case 4:
                    flags = value ? (ushort) (flags | 0x0008) : (ushort) (flags & unchecked((ushort) ~0x0008));
                    break;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        void SetEncryption(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    flags = value ? (ushort) (flags | 0x0040) : (ushort) (flags & unchecked((ushort) ~0x0040));
                    break;
                case 4:
                    flags = value ? (ushort) (flags | 0x0004) : (ushort) (flags & unchecked((ushort) ~0x0004));
                    break;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        void SetUnsynchronisation(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    break;
                case 4:
                    flags = value ? (ushort) (flags | 0x0002) : (ushort) (flags & unchecked((ushort) ~0x0002));
                    break;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }

        void SetDataLength(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    break;
                case 4:
                    flags = value ? (ushort) (flags | 0x0001) : (ushort) (flags & unchecked((ushort) ~0x0001));
                    break;
                default:
                    throw new InvalidOperationException($"ID3v2 Version {_version} is not supported.");
            }
        }
    }
}