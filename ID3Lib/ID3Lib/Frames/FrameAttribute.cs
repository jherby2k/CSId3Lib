// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using JetBrains.Annotations;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Define the type of frame
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FrameAttribute : Attribute
    {
        /// <summary>
        /// Get the frameId
        /// </summary>
        [NotNull]
        public string FrameId { get; }

        /// <summary>
        /// The frameId represented
        /// </summary>
        /// <param name="frameId">a frameId</param>
        public FrameAttribute([NotNull] string frameId)
        {
            FrameId = frameId ?? throw new ArgumentNullException("frameId");
        }
    }
}
