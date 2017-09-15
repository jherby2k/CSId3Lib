// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Id3Lib.Frames
{
    #region Enums
    /// <summary>
    /// Types of images
    /// (technically you can have a picture of each type in a single file)
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "The image type is a byte")]
    public enum PictureTypeCode : byte
    {
        /// <summary>
        /// Other
        /// </summary>
        Other = 0x00,
        /// <summary>
        /// 32x32 pixels 'file icon' (PNG only)
        /// </summary>
        Icon = 0x01,
        /// <summary>
        /// Other file icon
        /// </summary>
        OtherIcon = 0x02,
        /// <summary>
        /// Cover (front)
        /// </summary>
        CoverFront = 0x03,
        /// <summary>
        /// Cover (back)
        /// </summary>
        CoverBack = 0x04,
        /// <summary>
        /// Leaflet page
        /// </summary>
        Leaflet = 0x05,
        /// <summary>
        /// Media (e.g. label side of CD)
        /// </summary>
        Media = 0x06,
        /// <summary>
        /// Lead artist/lead performer/soloist
        /// </summary>
        LeadArtist = 0x07,
        /// <summary>
        /// Artist/performer
        /// </summary>
        Artist = 0x08,
        /// <summary>
        /// Conductor
        /// </summary>
        Conductor = 0x09,
        /// <summary>
        /// Band/Orchestra
        /// </summary>
        Orchestra = 0x0A,
        /// <summary>
        /// Composer
        /// </summary>
        Composer = 0x0B,
        /// <summary>
        /// Lyricist/text writer
        /// </summary>
        Lyricist = 0x0C,
        /// <summary>
        /// Recording Location
        /// </summary>
        Location = 0x0D,
        /// <summary>
        /// During recording
        /// </summary>
        Recording = 0x0E,
        /// <summary>
        /// During performance
        /// </summary>
        Performance = 0x0F,
        /// <summary>
        /// Movie/video screen capture
        /// </summary>
        Movie = 0x10,
        /// <summary>
        /// A bright coloured fish
        /// </summary>
        Fish = 0x11,
        /// <summary>
        /// Illustration
        /// </summary>
        Illustration = 0x12,
        /// <summary>
        /// Band/artist logotype
        /// </summary>
        BandLogo = 0x13,
        /// <summary>
        /// Publisher/Studio logotype
        /// </summary>
        StudioLogo = 0x14,
    };
    #endregion

    /// <summary>
    /// Picture Frame
    /// </summary>
    [Frame("APIC")]
    public class FramePicture : FrameBase, IFrameDescription
    {
        #region Fields
        private TextCode _textEncoding;
        private string _mime;
        private PictureTypeCode _pictureType;
        private string _description;
        private byte[] _pictureData;
        #endregion

        #region Constructors
        /// <summary>
        /// Picture Frame
        /// </summary>
        public FramePicture(string frameId)
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
        /// Picture MIME type
        /// </summary>
        public string Mime
        {
            get { return _mime; }
            set { _mime = value; }
        }

        /// <summary>
        /// Description of the picture
        /// </summary>
        public PictureTypeCode PictureType
        {
            get { return _pictureType; }
            set { _pictureType = value; }
        }

        /// <summary>
        /// Description of the picture
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        /// Binary data representing the picture
        /// </summary>
        public byte[] PictureData
        {
            get { return _pictureData; }
            set { _pictureData = value; }
        }

        /// <summary>
        /// Image of the picture
        /// </summary>
        public Image<Rgba32> Picture
        {
            get
            {
                return Image.Load(new MemoryStream(_pictureData, false));
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                _pictureData = value.SavePixelData();
                _mime = Image.DetectFormat(_pictureData).DefaultMimeType;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load from binary data a picture frame
        /// </summary>
        /// <param name="frame">picture binary representation</param>
        public override void Parse(byte[] frame)
        {
            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;
            _mime = TextBuilder.ReadASCII(frame, ref index);
            _pictureType = (PictureTypeCode)frame[index];
            index++;
            _description = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _pictureData = Memory.Extract(frame, index, frame.Length - index);
        }
        /// <summary>
        ///  Save picture frame to binary data
        /// </summary>
        /// <returns>picture binary representation</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
            writer.Write(TextBuilder.WriteASCII(_mime));
            writer.Write((byte)_pictureType);
            writer.Write(TextBuilder.WriteText(_description, _textEncoding));
            writer.Write(_pictureData);
            return buffer.ToArray();
        }

        /// <summary>
        /// Get a description of the picture frame
        /// </summary>
        /// <returns>Picture description</returns>
        public override string ToString()
        {
            return _description;
        }
        #endregion
    }
}
