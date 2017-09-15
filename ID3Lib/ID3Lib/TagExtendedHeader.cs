// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using Id3Lib.Exceptions;

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
	public class TagExtendedHeader
	{
		#region Fields
		private uint _size;
		private byte[] _extendedHeader;
		#endregion

		#region Properties
		/// <summary>
		/// Get the size of the extended header
		/// </summary>
		public uint Size
		{
			get{return _size;}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Load the ID3 extended header from a stream
		/// </summary>
		/// <param name="stream">Binary stream containing a ID3 extended header</param>
		public void Deserialize(Stream stream)
		{
            if (stream == null)
                throw new ArgumentNullException("stream");

			var reader = new BinaryReader(stream);
			_size = Swap.UInt32(Sync.UnsafeBigEndian(reader.ReadUInt32()));
			if(_size < 6)
                throw new InvalidFrameException("Corrupt id3 extended header.");
			
			// TODO: implement the extended header, copy for now since it's optional
			_extendedHeader = new Byte[_size];
			stream.Read(_extendedHeader,0,(int)_size);
		}

		/// <summary>
		/// Save the ID3 extended header from a stream
		/// </summary>
		/// <param name="stream">Binary stream containing a ID3 extended header</param>
		public void Serialize(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			// TODO: implement the extended header, for now write the original header
			writer.Write(_extendedHeader);
		}
		#endregion
	}
}
