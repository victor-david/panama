using Restless.Panama.Resources;
using System.IO;

namespace Restless.Panama.Core
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Throws an <see cref="IOException"/>
        /// </summary>
        public static void ThrowFileNotFound(string parm1, string parm2)
        {
            throw new IOException(string.Format(Error.FileNotFoundFormat, parm1, parm2));
        }
    }
}