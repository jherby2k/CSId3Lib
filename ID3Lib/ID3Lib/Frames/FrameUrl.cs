// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Manages predefined URL W*** (not WXXX) Frames
    /// </summary>
    /// <remarks>
    /// URL               text string
    /// </remarks>
    [PublicAPI]
    [Frame("W")]
    public class FrameUrl : FrameBase
    {
        [CanBeNull] Uri _uri;

        /// <summary>
        /// The URL page location
        /// </summary>
        [NotNull]
        public string Url => _uri == null ? string.Empty : _uri.AbsoluteUri;

        [CanBeNull]
        public Uri Uri
        {
            get => _uri;
            set => _uri = value ?? throw new ArgumentNullException("value");
        }

        /// <summary>
        /// Create a URL frame
        /// </summary>
        /// <param name="frameId">Type of URL frame</param>
        public FrameUrl([NotNull] string frameId)
            : base(frameId)
        {
        }

        /// <summary>
        /// Parse the binary frame
        /// </summary>
        /// <param name="frame">binary frame</param>
        public override void Parse(byte[] frame)
        {
            //TODO: Handle this invalid tag
            if (frame.Length < 1)
                return;

            var url = TextBuilder.ReadTextEnd(frame, 0, TextCode.Ascii);
            if (!Uri.TryCreate(url, UriKind.Absolute, out _uri))
                _uri = null;
        }

        /// <summary>
        /// Create a binary frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write(TextBuilder.WriteTextEnd(Url, TextCode.Ascii));
                return buffer.ToArray();
            }
        }
        /// <summary>
        /// Default frame description
        /// </summary>
        /// <returns>URL text</returns>
        [NotNull]
        public override string ToString()
        {
            return Url;
        }
    }
}
