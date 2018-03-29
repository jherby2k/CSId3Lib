// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Id3Lib.Frames
{
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

    /// <summary>
    /// Picture Frame
    /// </summary>
    [PublicAPI]
    [Frame("APIC")]
    public class FramePicture : FrameBase, IFrameDescription
    {
        /// <summary>
        /// Type of text encoding
        /// </summary>
        public TextCode TextEncoding { get; set; } = TextCode.Ascii;

        /// <summary>
        /// Picture MIME type
        /// </summary>
        [NotNull]
        public string Mime { get; set; }

        /// <summary>
        /// Description of the picture
        /// </summary>
        public PictureTypeCode PictureType { get; set; }

        /// <summary>
        /// Description of the picture
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Binary data representing the picture
        /// </summary>
        [NotNull]
        public byte[] PictureData { get; set; }

        /// <summary>
        /// Image of the picture
        /// </summary>
        [NotNull]
        public Image<Rgba32> Picture
        {
            get => Image.Load(new MemoryStream(PictureData, false));
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                PictureData = value.SavePixelData();
                Mime = Image.DetectFormat(PictureData).DefaultMimeType;
            }
        }

        /// <summary>
        /// Picture Frame
        /// </summary>
        public FramePicture([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Load from binary data a picture frame
        /// </summary>
        /// <param name="frame">picture binary representation</param>
        public override void Parse(byte[] frame)
        {
            var index = 0;
            TextEncoding = (TextCode) frame[index++];
            Mime = TextBuilder.ReadASCII(frame, ref index);
            PictureType = (PictureTypeCode) frame[index++];
            Description = TextBuilder.ReadText(frame, ref index, TextEncoding);
            PictureData = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        ///  Save picture frame to binary data
        /// </summary>
        /// <returns>picture binary representation</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write((byte) TextEncoding);
                writer.Write(TextBuilder.WriteASCII(Mime));
                writer.Write((byte) PictureType);
                writer.Write(TextBuilder.WriteText(Description, TextEncoding));
                writer.Write(PictureData);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Get a description of the picture frame
        /// </summary>
        /// <returns>Picture description</returns>
        [NotNull]
        public override string ToString()
        {
            return Description;
        }
    }
}
