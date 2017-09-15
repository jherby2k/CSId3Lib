// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage unique identifier frames.
    /// </summary>
    /// <remarks>
    ///   This frame's purpose is to be able to identify the audio file in a
    ///   database, that may provide more information relevant to the content.
    /// </remarks>
    [Frame("UFID")]
    public class FrameUniqueIdentifier : FrameBase, IFrameDescription
    {
        #region Fields
        private string _description;
        private byte[] _identifer;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a FrameGEOB frame.
        /// </summary>
        /// <param name="frameId">ID3v2 UFID frame</param>
        public FrameUniqueIdentifier(string frameId)
            : base(frameId)
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// Frame description
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Binary representation of the object
        /// </summary>
        public byte[] Identifier
        {
            get { return _identifer; }
            set
            {
                if (value.Length > 64)
                    throw new ArgumentOutOfRangeException("value", "The identifier can't be more than 64 bytes");
                _identifer = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse the binary UFID frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            int index = 0;
            _description = TextBuilder.ReadASCII(frame, ref index);
            _identifer = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary UFID frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write(TextBuilder.WriteASCII(_description));
            writer.Write(_identifer);
            return buffer.ToArray();
        }

        /// <summary>
        /// Unique Tag Identifier description 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _description;
        }
        #endregion
    }
}
