// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manages Text frames
    /// </summary>
    /// <remarks>
    /// The <b>FrameText</b> class handles the frames of text information these 
    /// are often the most important frames, containing information like artist,
    /// album and more. There may only be one text information frame of its kind in an tag.
    /// 
    /// Text encoding        $xx
    /// Information          text string(s) according to encoding
    /// </remarks>
    [Frame("T")]
    public class FrameText : FrameBase
    {
        #region Fields
        private string _text;
        private TextCode _textEncoding;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a FrameText frame.
        /// </summary>
        /// <param name="frameId">ID3v2 type of text frame</param>
        public FrameText(string frameId)
            : base(frameId)
        {
            _textEncoding = TextCode.Ascii;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the type of text encoding, the frame is using.
        /// </summary>
        public TextCode TextCode
        {
            get { return _textEncoding; }
            set { _textEncoding = value; }
        }

        /// <summary>
        /// Get or Set the text of the frame
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse the text binary frame.
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;
            _text = TextBuilder.ReadTextEnd(frame, index, _textEncoding);
        }

        /// <summary>
        /// Create a text binary frame.
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
            writer.Write(TextBuilder.WriteTextEnd(_text, _textEncoding));
            return buffer.ToArray();
        }

        /// <summary>
        /// Default Frame description.
        /// </summary>
        /// <returns>Text of the frame</returns>
        public override string ToString()
        {
            return _text;
        }
        #endregion
    }
}
