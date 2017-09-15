// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage play counter frames.
    /// </summary>
    /// <remarks>
    ///   This frame's purpose is to be able to identify the audio file in a
    ///   database, that may provide more information relevant to the content.
    /// </remarks>
    [Frame("PCNT")]
    public class FramePlayCounter : FrameBase
    {
        #region Fields
        private byte[] _counter = new byte[] { 0 };
        #endregion

        #region Constructors
        /// <summary>
        /// Create a Play Counter frame.
        /// </summary>
        /// <param name="frameId">ID3v2 PCNT frame</param>
        public FramePlayCounter(string frameId)
            : base(frameId)
        {

        }
        #endregion

        #region Properties

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
        /// Parse the binary PCNT frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            int index = 0;
            _counter = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary PCNT frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
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
