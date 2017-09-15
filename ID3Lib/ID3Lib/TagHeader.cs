// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Id3Lib.Exceptions;

namespace Id3Lib
{
    /// <summary>
    /// Manages the ID3v2 tag header.
    /// </summary>
    /// <remarks>
    ///  The <b>Header</b> class manages the first part of the ID3v2 tag that is the first ten bytes
    ///  of the ID3v1 tag.
    /// </remarks>
    public class TagHeader
    {
        #region Fields

        private byte _id3Version = 4;
        private byte _id3Revision /*= 0*/;
        private byte _id3Flags /*= 0*/;
        private uint _id3RawSize /*= 0*/;
        private uint _paddingSize /*= 0*/;

        private static readonly byte[] _id3 = { 0x49, 0x44, 0x33 }; //"ID3" tag
        private static readonly byte[] _3di = { 0x33, 0x44, 0x49 }; //"3DI" footer tag

        #endregion

        #region Properties

        /// <summary>
        /// Get the size of the header only.
        /// </summary>
        public static uint HeaderSize
        {
            get { return 10; } // ID3 Header size is fixed
        }

        /// <summary>
        /// Get or set ID3v2 major version number.
        /// </summary>
        public byte Version
        {
            get { return _id3Version; }
            set { _id3Version = value; }
        }
        /// <summary>
        /// Get or set the ID3v2 revision number.
        /// </summary>
        public byte Revision
        {
            get { return _id3Revision; }
            set { _id3Revision = value; }
        }
        /// <summary>
        /// Get or set the ID3v2 frames size, i.e. the tag excluding header and footer.
        /// </summary>
        public uint TagSize
        {
            get { return _id3RawSize; }
            set { _id3RawSize = value; }
        }

        /// <summary>
        /// Get the minimum complete ID3v2 tag size, 
        /// including header and footer but not including any padding.
        /// </summary>
        public uint TagSizeWithHeaderFooter
        {
            get
            {
                return _id3RawSize                  // frame data
                    + HeaderSize                    // the ID3v2 header
                    + (Footer ? HeaderSize : 0);    // optional footer
            }
        }

        /// <summary>
        /// Get or set if un-synchronisation is applied on all frames.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unsync")]
        public bool Unsync
        {
            get { return (_id3Flags & 0x80) > 0; }
            set
            {
                if (value == true)
                {
                    _id3Flags |= 0x80;
                }
                else
                {
                    unchecked { _id3Flags &= (byte)~(0x80); }
                }
            }
        }
        /// <summary>
        /// Get or set if the header is followed by an extended header.
        /// </summary>
        public bool ExtendedHeader
        {
            get { return (_id3Flags & 0x40) > 0; }
            set
            {
                if (value == true)
                {
                    _id3Flags |= 0x40;
                }
                else
                {
                    unchecked { _id3Flags &= (byte)~(0x40); }
                }
            }
        }
        /// <summary>
        /// Get or set if the tag is experimental stage.
        /// </summary>
        /// <remarks>
        /// This flag shall always be set when the tag is in an experimental stage.
        /// </remarks>
        public bool Experimental
        {
            get { return (_id3Flags & 0x20) > 0; }
            set
            {
                if (value == true)
                {
                    _id3Flags |= 0x20;
                }
                else
                {
                    unchecked { _id3Flags &= (byte)~(0x20); }
                }
            }
        }
        /// <summary>
        /// Get or set if a footer is present at the end of the tag.
        /// </summary>
        /// <remarks>
        /// Can't be used simultaneously with the frame padding they are mutually exclusive.
        /// </remarks>
        public bool Footer
        {
            get
            {
                return (_id3Flags & 0x10) > 0;
            }
            set
            {
                if (value == true)
                {
                    _id3Flags |= 0x10;
                    _paddingSize = 0;
                }
                else
                {
                    unchecked { _id3Flags &= (byte)~(0x10); }
                }
            }
        }

        /// <summary>
        /// Get if padding is applied on the tag.
        /// </summary>
        /// <remarks>
        /// Can't be used simultaneously with the frame footer they are mutually exclusive.
        /// </remarks>
        public bool Padding
        {
            get
            {
                return _paddingSize > 0;
            }
        }

        /// <summary>
        /// Get or set the padding size, and thus padding.
        /// </summary>
        public uint PaddingSize
        {
            set
            {
                Debug.Assert(value >= 0);

                if (value > 0)
                {
                    Footer = false;
                }
                _paddingSize = value;
            }
            get
            {
                return _paddingSize;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Save header into the stream.
        /// </summary>
        /// <param name="stream">Stream to save header</param>
        public void Serialize(Stream stream)
        {
            // open a writer on the underlying stream but don't use 'using'
            // or Dispose will close the underlying stream at the end
            BinaryWriter writer = new BinaryWriter(stream);

            //TODO: Validate version and revision we support
            writer.Write(_id3);         // ID3v2/file identifier
            writer.Write(_id3Version);  // ID3v2 version, e.g. 3 or 4
            writer.Write(_id3Revision); // ID3v2 revision, e.g. 0
            writer.Write(_id3Flags);    // ID3v2 flags
            writer.Write(Swap.UInt32(Sync.Safe(_id3RawSize + _paddingSize)));
        }

        /// <summary>
        /// Save corresponding footer into the stream.
        /// </summary>
        /// <param name="stream">Stream to save header</param>
        public void SerializeFooter(Stream stream)
        {
            // open a writer on the underlying stream but don't use 'using'
            // or Dispose will close the underlying stream at the end
            BinaryWriter writer = new BinaryWriter(stream);

            //TODO: Validate version and revision we support
            writer.Write(_3di);         // ID3v2/file footer identifier; ID3 backwards.
            writer.Write(_id3Version);  // ID3v2 version, e.g. 3 or 4
            writer.Write(_id3Revision); // ID3v2 revision, e.g. 0
            writer.Write(_id3Flags);    // ID3v2 flags
            writer.Write(Swap.UInt32(Sync.Safe(_id3RawSize)));
        }

        /// <summary>
        /// Load header from the stream.
        /// </summary>
        /// <param name="stream">Stream to load header</param>
        public void Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            byte[] idTag = new byte[3];

            // Read the tag identifier
            reader.Read(idTag, 0, 3);
            if (Memory.Compare(_id3, idTag) == false)
                throw new TagNotFoundException("ID3v2 tag identifier was not found");

            // Get the id3v2 version byte
            _id3Version = reader.ReadByte();
            if (_id3Version == 0xff)
                throw new InvalidTagException("Corrupt header, invalid ID3v2 version.");

            // Get the id3v2 revision byte
            _id3Revision = reader.ReadByte();
            if (_id3Revision == 0xff)
                throw new InvalidTagException("Corrupt header, invalid ID3v2 revision.");

            // Get the id3v2 flag byte, only read what I understand
            _id3Flags = (byte)(0xf0 & reader.ReadByte());
            // Get the id3v2 size, swap and un-sync the integer
            _id3RawSize = Swap.UInt32(Sync.UnsafeBigEndian(reader.ReadUInt32()));
            if (_id3RawSize == 0)
                throw new InvalidTagException("Corrupt header, tag size can't be zero.");

        }
        #endregion

    }
}
