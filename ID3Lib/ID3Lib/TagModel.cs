// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Id3Lib.Frames;
using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// Manages an ID3v2 tag as an object representation. 
    /// </summary>
    /// <remarks>
    /// The <b>FrameModel</b> class represents a ID3v2 tag, it contains a <see cref="Header"/> that
    /// handles the tag header, an <see cref="ExtendedHeader"/> that it is optional and 
    /// stores the frames.
    /// </remarks>
    [PublicAPI]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "It is more than a collection as it has extra behaviour")]
    public class TagModel : Collection<FrameBase>
    {
        /// <summary>
        /// id3v2 tags can not have "no frames"
        /// </summary>
        public bool IsValid => Count > 0;

        /// <summary>
        /// Gets the header.
        /// </summary>
        [NotNull]
        public TagHeader Header { get; } = new TagHeader();

        /// <summary>
        /// Gets the extended header.
        /// </summary>
        [NotNull]
        public TagExtendedHeader ExtendedHeader { get; } = new TagExtendedHeader();

        protected override void InsertItem(int index, [NotNull] FrameBase item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.FrameId == null || item.FrameId.Length != 4)
                throw new InvalidOperationException("The frame id is invalid");
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, [NotNull] FrameBase item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.FrameId == null || item.FrameId.Length != 4)
                throw new InvalidOperationException("The frame id is invalid");
            base.SetItem(index, item);
        }

        /// <summary>
        /// Add a range of frames
        /// </summary>
        /// <param name="frames">the frames to add</param>
        public void AddRange([NotNull] IEnumerable<FrameBase> frames)
        {
            if (frames == null)
                throw new ArgumentNullException("frames");

            //add each frame to the collection
            foreach (var frame in frames)
                Add(frame);
        }

        /// <summary>
        /// predict the size of the frames on disk (without any padding)
        /// by streaming the tag to a dummy stream, which updates the stored size.
        /// </summary>
        /// <remarks>
        /// Although the the padding is streamed out, 
        /// the size isn't added on to Header.TagSize
        /// </remarks>
        public void UpdateSize()
        {
            if (!IsValid)
                Header.TagSize = 0; // clear the TagSize stored in the tagModel

            // TODO: there must be a better way of obtaining this!!
            using (Stream stream = new MemoryStream())
                TagManager.Serialize(this, stream);
        }
    }
}
