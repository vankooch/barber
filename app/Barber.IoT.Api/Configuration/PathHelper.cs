﻿namespace Barber.IoT.Api.Configuration
{
    using System;
    using System.IO;

    public static class PathHelper
    {
        public static string? ExpandPath(string path)
        {
            if (path == null)
            {
                return null;
            }

            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? $".{Path.DirectorySeparatorChar}", path);
            }

            return path;
        }
    }
}
