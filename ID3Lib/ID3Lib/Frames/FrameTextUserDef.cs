// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System.IO;
using System.Text;
using JetBrains.Annotations;

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
    [PublicAPI]
    [Frame("TXXX")]
    public class FrameTextUserDef : FrameBase, IFrameDescription
	{
		/// <summary>
		/// Get or set the type of text encoding the frame is using.
		/// </summary>
		public TextCode TextCode { get; set; } = TextCode.Ascii;

	    /// <summary>
		/// Get or set the description of the frame contents.
		/// </summary>
	    public string Description { get; set; }

	    /// <summary>
		/// Get or set the main text data.
		/// </summary>
		[NotNull]
	    public string Text { get; set; }

	    /// <summary>
	    /// Create a FrameLCText frame.
	    /// </summary>
	    /// <param name="frameId">ID3v2 type of text frame</param>
	    public FrameTextUserDef([NotNull] string frameId)
	        : base(frameId)
	    {
	    }

		/// <summary>
		/// Parse binary data unsynchronised lyrics/comment frame.
		/// </summary>
		/// <param name="frame">binary frame data</param>
		public override void Parse(byte[] frame)
		{
			var index = 0;
			TextCode = (TextCode)frame[index++];

            //TODO: Invalid tag, may be legacy.
            if (frame.Length - index < 3)
                return;

			Description = TextBuilder.ReadText(frame, ref index, TextCode);
			Text = TextBuilder.ReadTextEnd(frame, index, TextCode);
		}

		/// <summary>
		/// Create binary data from unsynchronised lyrics/comment frame 
		/// </summary>
		/// <returns>binary frame data</returns>
		public override byte[] Make()
		{
            using (var buffer = new MemoryStream())
		    using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
		    {
		        writer.Write((byte) TextCode);
		        writer.Write(TextBuilder.WriteText(Description, TextCode));
		        writer.Write(TextBuilder.WriteTextEnd(Text, TextCode));
		        return buffer.ToArray();
		    }
		}

		/// <summary>
		/// Default frame description.
		/// </summary>
		/// <returns>unsynchronised lyrics/comment text</returns>
		[NotNull]
		public override string ToString()
		{
			return Text;
		}
	}
}
