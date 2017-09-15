// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage popularimeter frames.
    /// </summary>
    /// <remarks>
    ///   The purpose of this frame is to specify how good an audio file is.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Popularimeter")]
    [Frame("POPM")]
    public class FramePopularimeter : FrameBase, IFrameDescription
    {
        #region Fields
        private string _description;
        byte _rating;
        private byte[] _counter = new byte[] { 0 };
        #endregion

        #region Constructors
        /// <summary>
        /// Create a Play Counter frame.
        /// </summary>
        /// <param name="frameId">ID3v2 POPM frame</param>
        public FramePopularimeter(string frameId)
            : base(frameId)
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// The rating is 1-255 where 1 is worst and 255 is best. 0 is unknown.
        /// </summary>
        public byte Rating
        {
            get { return _rating; }
            set { _rating = value; }
        }

        /// <summary>
        /// Email address
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Get the number of times the song has been played
        /// </summary>
        public ulong Counter
        {
            get { return Memory.ToInt64(_counter); }
            set { _counter = Memory.GetBytes(value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse the binary POPM frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            int index = 0;
            _description = TextBuilder.ReadASCII(frame, ref index);
            _rating = frame[index];
            index++;
            _counter = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary POPM frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write(TextBuilder.WriteASCII(_description));
            writer.Write(_rating);
            writer.Write(_counter);
            return buffer.ToArray();
        }

        /// <summary>
        /// Unique Tag Identifer description 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return null;
        }
        #endregion
    }
}
