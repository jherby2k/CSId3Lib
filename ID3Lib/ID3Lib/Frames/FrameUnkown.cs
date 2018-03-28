// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.

using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Container for unknown frames.
    /// </summary>
    /// <remarks>
    /// The <b>FrameUnknown</b> class handles unknown frames so they can be restored
    /// or discarded later.
    /// </remarks>
    [PublicAPI]
    public class FrameUnknown : FrameBase
	{
	    [NotNull] byte[] _data;

		/// <summary>
		/// Create an unknown frame object.
		/// </summary>
        /// <param name="frameId">ID3v2 type of unknown frame</param>
        internal FrameUnknown([NotNull] string frameId)
            : base(frameId)
		{
		}

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
		[NotNull]
		public override string ToString()
		{
            return "Unknown ID3 frameId";
		}
	}
}
