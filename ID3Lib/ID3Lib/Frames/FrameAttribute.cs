// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Define the type of frame
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FrameAttribute : Attribute
    {
        #region Fields
        string _frameId;
        #endregion

        /// <summary>
        /// The frameId represented
        /// </summary>
        /// <param name="frameId">a frameId</param>
        public FrameAttribute(string frameId)
        {
            if (frameId == null)
                throw new ArgumentNullException("frameId");

            _frameId = frameId;

        }

        /// <summary>
        /// Get the frameId
        /// </summary>
        public string FrameId
        {
            get { return _frameId; }
        }
    }
}
