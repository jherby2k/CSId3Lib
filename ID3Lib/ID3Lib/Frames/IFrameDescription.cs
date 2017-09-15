using System;
using System.Collections.Generic;
using System.Text;

namespace Id3Lib.Frames
{
    /// <summary>
    /// Frame description
    /// </summary>
    /// <remarks>
    /// Frames that have a description must include this interface, 
    /// it will be used to make a validation that the frames are unique by description;
    /// </remarks>
    public interface IFrameDescription
    {

        /// <summary>
        /// Description of the frame, it means different things on the specific frame.
        /// </summary>
        string Description
        {
            get;
            set;
        }
    }
}
