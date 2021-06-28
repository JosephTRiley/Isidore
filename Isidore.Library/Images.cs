using System.Drawing;
using Isidore.Library.Properties;

namespace Isidore.Library
{
    /// <summary>
    /// Images library
    /// </summary>
    public class Images
    {
        /// <summary>
        /// Returns a Mono-color bitmap of a capital R
        /// </summary>
        /// <returns> "R" bitmap </returns>
        public static Bitmap R()
        {
            Bitmap R = Resources.R;
            return R;
        }
    }
}
