// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Id3Lib.Exceptions;
using Id3Lib.Frames;
using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// Builder factory that creates frames
    /// </summary>
    ///<remarks>
    /// The FrameFactory class creates the correct frame based on the four bytes each frame uses
    /// to describe its type, also a description of the frame is added.
    ///</remarks>
    [PublicAPI]
    public static class FrameFactory
    {
        [NotNull] static readonly Dictionary<string, Type> _frames = new Dictionary<string, Type>();

        /// <summary>
        /// Default constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static FrameFactory()
        {
            // Scan the assembly for frames configured
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            foreach (var frameAttribute in (FrameAttribute[]) type.GetCustomAttributes<FrameAttribute>(false))
                _frames.Add(frameAttribute.FrameId, type);
        }

        /// <summary>
        /// Builds a frame base, based on the frame tag type.
        /// </summary>
        /// <param name="frameId">The ID3v2 tag frame id</param>
        /// <returns>Frame required for the frame type</returns>
        [NotNull]
        public static FrameBase Build([NotNull] string frameId)
        {
            if (frameId == null)
                throw new ArgumentNullException("frameId");

            if (frameId.Length != 4)
                throw new InvalidTagException($"Invalid frame type: '{frameId}', it must be 4 characters long.");

            //Try to find the most specific frame first
            if (_frames.TryGetValue(frameId, out var type))
                return (FrameBase) Activator.CreateInstance(type, frameId);

            //Get the T*** or U*** frame, they are all identical except for the user defined frames 'TXXX' and 'WXXX'.
            if (_frames.TryGetValue(frameId.Substring(0, 1), out type))
                return (FrameBase) Activator.CreateInstance(type, frameId);

            // Unknown tag, used as a container for unknown frames
            return new FrameUnknown(frameId);
        }
    }
}
