// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using System.Text;
using Id3Lib.Exceptions;
using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// ID3 Extended Header
    /// </summary>
    /// <remarks>
    /// The extended header contains information that can provide further
    /// insight in the structure of the tag, but is not vital to the correct
    /// parsing of the tag information; hence the extended header is optional.
    /// </remarks>
    [PublicAPI]
    public class TagExtendedHeader
	{
        [CanBeNull] byte[] _extendedHeader;

        /// <summary>
        /// Get the size of the extended header
        /// </summary>
        public uint Size { get; private set; }

        /// <summary>
        /// Load the ID3 extended header from a stream
        /// </summary>
        /// <param name="stream">Binary stream containing a ID3 extended header</param>
        public void Deserialize([NotNull] Stream stream)
		{
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                Size = Swap.UInt32(Sync.UnsafeBigEndian(reader.ReadUInt32()));
			if (Size < 6)
                throw new InvalidFrameException("Corrupt id3 extended header.");
			
			// TODO: implement the extended header, copy for now since it's optional
			_extendedHeader = new byte[Size];
		    stream.Read(_extendedHeader, 0, (int) Size);
		}

		/// <summary>
		/// Save the ID3 extended header from a stream
		/// </summary>
		/// <param name="stream">Binary stream containing a ID3 extended header</param>
		public void Serialize([NotNull] Stream stream)
		{
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                // TODO: implement the extended header, for now write the original header
                writer.Write(_extendedHeader);
		}
	}
}
