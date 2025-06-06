﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

#if WINDOWS_BUILD

using ImageMagick;
using Xunit;

namespace Magick.NET.Tests;

public partial class OpenCLDeviceTests
{
    public partial class TheNameProperty
    {
        [Fact]
        public void ShouldNotBeNull()
        {
            foreach (var device in OpenCL.Devices)
            {
                Assert.NotNull(device.Name);
            }
        }
    }
}

#endif
