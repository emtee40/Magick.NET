﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;

namespace ImageMagick;

/// <summary>
/// Represents the density of an image.
/// </summary>
public sealed class Density : IEquatable<Density?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Density"/> class with the density set to inches.
    /// </summary>
    /// <param name="xy">The x and y.</param>
    public Density(double xy)
      : this(xy, xy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Density"/> class.
    /// </summary>
    /// <param name="xy">The x and y.</param>
    /// <param name="units">The units.</param>
    public Density(double xy, DensityUnit units)
      : this(xy, xy, units)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Density"/> class with the density set to inches.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    public Density(double x, double y)
      : this(x, y, DensityUnit.PixelsPerInch)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Density"/> class.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="units">The units.</param>
    public Density(double x, double y, DensityUnit units)
    {
        X = x;
        Y = y;
        Units = units;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Density"/> class.
    /// </summary>
    /// <param name="value">Density specifications in the form: &lt;x&gt;x&lt;y&gt;[inch/cm] (where x, y are numbers).</param>
    public Density(string value)
        => Initialize(value);

    /// <summary>
    /// Gets the units.
    /// </summary>
    public DensityUnit Units { get; private set; }

    /// <summary>
    /// Gets the x resolution.
    /// </summary>
    public double X { get; private set; }

    /// <summary>
    /// Gets the y resolution.
    /// </summary>
    public double Y { get; private set; }

    /// <summary>
    /// Changes the density of the instance to the specified units.
    /// </summary>
    /// <param name="units">The units to use.</param>
    /// <returns>A new <see cref="Density"/> with the specified units.</returns>
    public Density ChangeUnits(DensityUnit units)
    {
        if (Units == units || Units == DensityUnit.Undefined || units == DensityUnit.Undefined)
            return new Density(X, Y, units);
        else if (Units == DensityUnit.PixelsPerCentimeter && units == DensityUnit.PixelsPerInch)
            return new Density(X * 2.54, Y * 2.54, units);
        else
            return new Density(X / 2.54, Y / 2.54, units);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the <see cref="Density"/>.
    /// </summary>
    /// <param name="obj">The object to compare this <see cref="Density"/> with.</param>
    /// <returns>True when the specified object is equal to the <see cref="Density"/>.</returns>
    public override bool Equals(object? obj)
        => Equals(obj as Density);

    /// <summary>
    /// Determines whether the specified <see cref="Density"/> is equal to the current <see cref="Density"/>.
    /// </summary>
    /// <param name="other">The <see cref="Density"/> to compare this <see cref="Density"/> with.</param>
    /// <returns>True when the specified <see cref="Density"/> is equal to the current <see cref="Density"/>.</returns>
    public bool Equals(Density? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return
          X == other.X &&
          Y == other.Y &&
          Units == other.Units;
    }

    /// <summary>
    /// Serves as a hash of this type.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode()
        =>
          X.GetHashCode() ^
          Y.GetHashCode() ^
          Units.GetHashCode();

    /// <summary>
    /// Returns a string that represents the current <see cref="Density"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="Density"/>.</returns>
    public override string ToString()
        => ToString(Units);

    /// <summary>
    /// Returns a string that represents the current <see cref="Density"/>.
    /// </summary>
    /// <param name="units">The units to use.</param>
    /// <returns>A string that represents the current <see cref="Density"/>.</returns>
    public string ToString(DensityUnit units)
    {
        if (Units == units || units == DensityUnit.Undefined)
            return ToString(X, Y, units);
        else if (Units == DensityUnit.PixelsPerCentimeter && units == DensityUnit.PixelsPerInch)
            return ToString(X * 2.54, Y * 2.54, units);
        else
            return ToString(X / 2.54, Y / 2.54, units);
    }

    private static string ToString(double x, double y, DensityUnit units)
    {
        var result = string.Format(CultureInfo.InvariantCulture, "{0}x{1}", x, y);

        return units switch
        {
            DensityUnit.PixelsPerCentimeter => result + " cm",
            DensityUnit.PixelsPerInch => result + " inch",
            _ => result,
        };
    }

    private void Initialize(string value)
    {
        Throw.IfNullOrEmpty(value);

        var values = value.Split(' ');
        Throw.IfTrue(values.Length > 2, nameof(value), "Invalid density specified.");

        if (values.Length == 2)
        {
            if (values[1].Equals("cm", StringComparison.OrdinalIgnoreCase))
                Units = DensityUnit.PixelsPerCentimeter;
            else if (values[1].Equals("inch", StringComparison.OrdinalIgnoreCase))
                Units = DensityUnit.PixelsPerInch;
            else
                throw new ArgumentException("Invalid density specified.", nameof(value));
        }

        var xyValues = values[0].Split('x');
        Throw.IfTrue(xyValues.Length > 2, nameof(value), "Invalid density specified.");

        Throw.IfFalse(double.TryParse(xyValues[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var x), nameof(value), "Invalid density specified.");

        double y;
        if (xyValues.Length == 1)
            y = x;
        else
            Throw.IfFalse(double.TryParse(xyValues[1], NumberStyles.Number, CultureInfo.InvariantCulture, out y), nameof(value), "Invalid density specified.");

        X = x;
        Y = y;
    }
}
