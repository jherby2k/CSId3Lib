// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;

namespace Id3Lib.Frames
{
	/// <summary>
	/// Container for unknown frames.
	/// </summary>
	/// <remarks>
	/// The <b>FrameUnknown</b> class handles unknown frames so they can be restored
	/// or discarded later.
	/// </remarks>
    public class FrameUnknown : FrameBase
	{
		#region Fields
		private byte[] _data;
		#endregion

		#region Constructors
		/// <summary>
		/// Create an unknown frame object.
		/// </summary>
        /// <param name="frameId">ID3v2 type of unknown frame</param>
        internal FrameUnknown( string frameId )
            : base(frameId)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Set the binary frame
		/// </summary>
		/// <param name="frame">binary frame unknown</param>
		public override void Parse(byte[] frame)
		{
			_data = frame;
		}

		/// <summary>
		/// Get a binary frame
		/// </summary>
		/// <returns>binary frame unknown</returns>
		public override byte[] Make()
		{
			return _data;
		}
		/// <summary>
		/// Default Frame description
		/// </summary>
        /// <returns>Unknown ID3 frameId</returns>
		public override string ToString()
		{
            return "Unknown ID3 frameId";
		}
		#endregion
	}
}
