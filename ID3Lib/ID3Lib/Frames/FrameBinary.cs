// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manage general encapsulated objects.
    /// </summary>
    /// <remarks>
    /// The <b>FrameBinary</b> class handles GEOB ID3v2 frame types that can hold any type of file
    /// or binary data encapsulated.
    /// </remarks>
    [Frame("GEOB")]
    public class FrameBinary : FrameBase, IFrameDescription
    {
        #region Fields
        private TextCode _textEncoding;
        private string _mime;
        private string _fileName;
        private string _description;
        private byte[] _objectData;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a FrameGEOB frame.
        /// </summary>
        /// <param name="frameId">ID3v2 GEOB frame</param>
        public FrameBinary(string frameId)
            : base(frameId)
        {
            _textEncoding = TextCode.Ascii;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Type of text encoding
        /// </summary>
        public TextCode TextEncoding
        {
            get { return _textEncoding; }
            set { _textEncoding = value; }
        }

        /// <summary>
        /// Text MIME type
        /// </summary>
        public string Mime
        {
            get { return _mime; }
            set { _mime = value; }
        }

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
        public byte[] ObjectData
        {
            get { return _objectData; }
            set { _objectData = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse the binary GEOB frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;
            _mime = TextBuilder.ReadASCII(frame, ref index);
            _fileName = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _description = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _objectData = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary GEOB frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
            writer.Write(TextBuilder.WriteASCII(_mime));
            writer.Write(TextBuilder.WriteText(_fileName, _textEncoding));
            writer.Write(TextBuilder.WriteText(_description, _textEncoding));
            writer.Write(_objectData);
            return buffer.ToArray();
        }

        /// <summary>
        /// GEOB frame description 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _description;
        }
        #endregion
    }
}
