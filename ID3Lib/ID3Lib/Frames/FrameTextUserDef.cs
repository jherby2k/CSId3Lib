// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;

namespace Id3Lib.Frames
{
	/// <summary>
    /// Manages User defined text TXXX Frames
	/// </summary>
    /// <remarks>
    /// Text encoding        $xx
    /// Content descriptor   text string according to encoding, $00 (00)
    /// text                 text string according to encoding
    /// </remarks>
    [Frame("TXXX")]
    public class FrameTextUserDef : FrameBase, IFrameDescription
	{
		#region Fields
		private string _contents;
		private string _text;
		private TextCode _textEncoding;
		#endregion

		#region Constructors
		/// <summary>
		/// Create a FrameLCText frame.
		/// </summary>
        /// <param name="frameId">ID3v2 type of text frame</param>
        public FrameTextUserDef(string frameId)
            : base(frameId)
		{
			_textEncoding = TextCode.Ascii;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Get or set the type of text encoding the frame is using.
		/// </summary>
		public TextCode TextCode
		{
			get { return _textEncoding;}
			set { _textEncoding = value;}
		}
		/// <summary>
		/// Get or set the description of the frame contents.
		/// </summary>
		public string Description
		{
			get{return _contents;}
			set{_contents = value;}
		}
		/// <summary>
		/// Get or set the main text data.
		/// </summary>
		public string Text
		{
			get{return _text;}
			set{_text = value;}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Parse binary data unsynchronised lyrics/comment frame.
		/// </summary>
		/// <param name="frame">binary frame data</param>
		public override void Parse(byte[] frame)
		{
			int index = 0;
			_textEncoding = (TextCode)frame[index];
			index++;

            //TODO: Invalid tag, may be legacy.
            if (frame.Length - index < 3)
                return;

			_contents = TextBuilder.ReadText(frame, ref index, _textEncoding);
			_text = TextBuilder.ReadTextEnd(frame, index, _textEncoding);
		}

		/// <summary>
		/// Create binary data from unsynchronised lyrics/comment frame 
		/// </summary>
		/// <returns>binary frame data</returns>
		public override byte[] Make()
		{
			MemoryStream buffer = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(buffer);
			writer.Write((byte)_textEncoding);
			writer.Write(TextBuilder.WriteText(_contents,_textEncoding));
			writer.Write(TextBuilder.WriteTextEnd(_text,_textEncoding));
			return buffer.ToArray();
		}

		/// <summary>
		/// Default frame description.
		/// </summary>
		/// <returns>unsynchronised lyrics/comment text</returns>
		public override string ToString()
		{
			return _text;
		}
		#endregion
	}
}
