// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage play counter frames.
    /// </summary>
    /// <remarks>
    ///   This frame's purpose is to be able to identify the audio file in a
    ///   database, that may provide more information relevant to the content.
    /// </remarks>
    [PublicAPI]
    [Frame("PCNT")]
    public class FramePlayCounter : FrameBase
    {
        [NotNull] byte[] _counter = { 0 };

        /// <summary>
        /// Get the number of times the song has been played
        /// </summary>
        public ulong Counter
        {
            get => Memory.ToInt64(_counter);
            set => _counter = Memory.GetBytes(value);
        }

        /// <summary>
        /// Create a Play Counter frame.
        /// </summary>
        /// <param name="frameId">ID3v2 PCNT frame</param>
        public FramePlayCounter([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Parse the binary PCNT frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            _counter = Memory.Extract(frame, 0, frame.Length);
        }

        /// <summary>
        /// Create a binary PCNT frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write(_counter);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Unique Tag Identifer description 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public override string ToString()
        {
            return string.Empty;
        }
    }
}
