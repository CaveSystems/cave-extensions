using System;

namespace Cave
{
    /// <summary>
    /// Provides flags for software revisions / versions.
    /// </summary>
    [Flags]
    public enum SoftwareFlags
    {
        /// <summary>
        /// No flags
        /// </summary>
        None = 0,

        /// <summary>
        /// Software suite (<see cref="Stable"/>, <see cref="Testing"/>)
        /// </summary>
        Suite = 0x0F,

        /// <summary>
        /// Software is stable
        /// </summary>
        Stable = 0x01,

        /// <summary>
        /// Software is testing
        /// </summary>
        Testing = 0x02,

        /// <summary>
        /// Software unstable trunk or branch
        /// </summary>
        Unstable = 0x03,

        /// <summary>
        /// Software configuration (<see cref="Release"/>, <see cref="Debug"/>)
        /// </summary>
        Configuration = 0xF0,

        /// <summary>
        /// Software with release configuration
        /// </summary>
        Release = 0x10,

        /// <summary>
        /// Software with debug configuration
        /// </summary>
        Debug = 0x20,

        /// <summary>
        /// Stable release version (end user version)
        /// </summary>
        Stable_Release = Stable | Release,

        /// <summary>
        /// Stable debug version (end user version for debugging and bug fixing)
        /// </summary>
        Stable_Debug = Stable | Debug,

        /// <summary>
        /// Testing release version (betatester version with newly implemented functionality)
        /// </summary>
        Testing_Release = Testing | Release,

        /// <summary>
        /// Testing debug version (betatester version with newly implemented functionality for debugging and bug fixing)
        /// </summary>
        Testing_Debug = Testing | Debug,
    }
}
