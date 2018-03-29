using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace Id3Lib.Tests
{
    static class Resources
    {
        internal static Stream GetResource([NotNull] string resource)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"ID3Lib.Tests.Resources.{resource}");

            if (stream == null)
                throw new ArgumentException("resource not found");

            return stream;
        }
    }
}
