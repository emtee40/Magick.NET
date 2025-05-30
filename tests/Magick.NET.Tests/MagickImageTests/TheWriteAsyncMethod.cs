﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using ImageMagick.Formats;
using Xunit;

namespace Magick.NET.Tests;

public partial class MagickImageTests
{
    public partial class TheWriteAsyncMethod
    {
        public class WithFileInfo
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenFileIsNull()
            {
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("file", () => image.WriteAsync((FileInfo)null!, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldUseTheFileExtension()
            {
                var settings = new MagickReadSettings
                {
                    Format = MagickFormat.Png,
                };
                using var input = new MagickImage(Files.CirclePNG, settings);

                using var tempFile = new TemporaryFile(".jpg");
                await input.WriteAsync(tempFile.File, TestContext.Current.CancellationToken);

                using var output = new MagickImage(tempFile.File);

                Assert.Equal(MagickFormat.Jpeg, output.Format);
            }
        }

        public class WithFileInfoAndMagickFormat
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenFileIsNull()
            {
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("file", () => image.WriteAsync((FileInfo)null!, MagickFormat.Bmp, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldUseTheSpecifiedFormat()
            {
                using var input = new MagickImage(Files.CirclePNG);

                using var tempFile = new TemporaryFile("foobar");
                await input.WriteAsync(tempFile.File, MagickFormat.Tiff, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Png, input.Format);

                using var output = new MagickImage(tempFile.File);

                Assert.Equal(MagickFormat.Tiff, output.Format);
            }
        }

        public class WithFileInfoAndWriteDefines
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenFileIsNull()
            {
                var defines = new JpegWriteDefines();
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("file", () => image.WriteAsync((FileInfo)null!, defines, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldThrowExceptionWhenWriteDefinesIsNull()
            {
                var file = new FileInfo(Files.CirclePNG);
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("defines", () => image.WriteAsync(file, null!, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldUseTheSpecifiedFormat()
            {
                var defines = new JpegWriteDefines
                {
                    DctMethod = JpegDctMethod.Fast,
                };
                using var input = new MagickImage(Files.CirclePNG);

                using var tempFile = new TemporaryFile("foobar");
                await input.WriteAsync(tempFile.File, defines, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Png, input.Format);

                using var output = new MagickImage();
                await output.ReadAsync(tempFile.File, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Jpeg, output.Format);
            }
        }

        public class WithFileName
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenFileIsNull()
            {
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("fileName", () => image.WriteAsync((string)null!, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldSyncTheExifProfile()
            {
                using var input = new MagickImage(Files.FujiFilmFinePixS1ProPNG);

                Assert.Equal(OrientationType.TopLeft, input.Orientation);

                input.Orientation = OrientationType.RightTop;

                using var memStream = new MemoryStream();
                await input.WriteAsync(memStream, TestContext.Current.CancellationToken);
                memStream.Position = 0;

                using var output = new MagickImage(Files.FujiFilmFinePixS1ProPNG);
                await output.ReadAsync(memStream, TestContext.Current.CancellationToken);

                Assert.Equal(OrientationType.RightTop, output.Orientation);

                var profile = output.GetExifProfile();

                Assert.NotNull(profile);

                var exifValue = profile.GetValue(ExifTag.Orientation);

                Assert.NotNull(exifValue);
                Assert.Equal((ushort)6, exifValue.Value);
            }
        }

        public class WithFileNameAndMagickFormat
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenFileIsNull()
            {
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("fileName", () => image.WriteAsync((string)null!, MagickFormat.Bmp, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldUseTheSpecifiedFormat()
            {
                using var input = new MagickImage(Files.CirclePNG);

                using var tempFile = new TemporaryFile("foobar");
                await input.WriteAsync(tempFile.File.FullName, MagickFormat.Tiff, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Png, input.Format);

                using var output = new MagickImage(tempFile.File.FullName);

                Assert.Equal(MagickFormat.Tiff, output.Format);
            }
        }

        public class WithFileNameAndWriteDefines
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenFileNameIsNull()
            {
                var defines = new JpegWriteDefines();
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("fileName", () => image.WriteAsync((string)null!, defines, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldThrowExceptionWhenWriteDefinesIsNull()
            {
                var file = new FileInfo(Files.CirclePNG);
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("defines", () => image.WriteAsync(file, null!, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldUseTheSpecifiedFormat()
            {
                var defines = new JpegWriteDefines
                {
                    DctMethod = JpegDctMethod.Fast,
                };

                using var input = new MagickImage(Files.CirclePNG);

                using var tempFile = new TemporaryFile("foobar");
                await input.WriteAsync(tempFile.File.FullName, defines, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Png, input.Format);

                using var output = new MagickImage();
                await output.ReadAsync(tempFile.File.FullName, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Jpeg, output.Format);
            }
        }

        public class WithStream
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenStreamIsNull()
            {
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("stream", () => image.WriteAsync((Stream)null!, TestContext.Current.CancellationToken));
            }
        }

        public class WithStreamAndMagickFormat
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenStreamIsNull()
            {
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("stream", () => image.WriteAsync((Stream)null!, MagickFormat.Bmp, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldUseTheSpecifiedFormat()
            {
                using var input = new MagickImage(Files.CirclePNG);

                using var memoryStream = new MemoryStream();
                using var stream = new NonSeekableStream(memoryStream);
                await input.WriteAsync(stream, MagickFormat.Tiff, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Png, input.Format);

                memoryStream.Position = 0;
                using var output = new MagickImage(stream);

                Assert.Equal(MagickFormat.Tiff, output.Format);
            }
        }

        public class WithStreamAndWriteDefines
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenStreamIsNull()
            {
                var defines = new JpegWriteDefines();
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("stream", () => image.WriteAsync((Stream)null!, defines, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldThrowExceptionWhenWriteDefinesIsNull()
            {
                using var stream = new MemoryStream();
                using var image = new MagickImage();

                await Assert.ThrowsAsync<ArgumentNullException>("defines", () => image.WriteAsync(stream, null!, TestContext.Current.CancellationToken));
            }

            [Fact]
            public async Task ShouldUseTheSpecifiedFormat()
            {
                var defines = new JpegWriteDefines
                {
                    DctMethod = JpegDctMethod.Fast,
                };

                using var input = new MagickImage(Files.CirclePNG);
                using var stream = new MemoryStream();
                await input.WriteAsync(stream, defines, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Png, input.Format);

                stream.Position = 0;
                using var output = new MagickImage();
                await output.ReadAsync(stream, TestContext.Current.CancellationToken);

                Assert.Equal(MagickFormat.Jpeg, output.Format);
            }
        }
    }
}
