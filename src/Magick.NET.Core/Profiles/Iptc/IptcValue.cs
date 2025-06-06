﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Text;

namespace ImageMagick;

/// <summary>
/// A value of the iptc profile.
/// </summary>
public sealed class IptcValue : IIptcValue
{
    private byte[] _data;
    private Encoding _encoding;

    internal IptcValue(IptcTag tag, byte[] value)
    {
        Throw.IfNull(value);

        Tag = tag;
        _data = value;
        _encoding = Encoding.UTF8;
    }

    internal IptcValue(IptcTag tag, string value)
    {
        Tag = tag;
        _encoding = Encoding.UTF8;
        _data = GetData(value);
    }

    /// <summary>
    /// Gets the tag of the iptc value.
    /// </summary>
    public IptcTag Tag { get; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value
    {
        get => _encoding.GetString(_data);
        set => _data = GetData(value);
    }

    /// <summary>
    /// Gets the length of the value.
    /// </summary>
    public int Length
        => _data.Length;

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="IptcValue"/>.
    /// </summary>
    /// <param name="obj">The object to compare this <see cref="IptcValue"/> with.</param>
    /// <returns>True when the specified object is equal to the current <see cref="IptcValue"/>.</returns>
    public override bool Equals(object? obj)
        => Equals(obj as IIptcValue);

    /// <summary>
    /// Determines whether the specified iptc value is equal to the current <see cref="IptcValue"/>.
    /// </summary>
    /// <param name="other">The iptc value to compare this <see cref="IptcValue"/> with.</param>
    /// <returns>True when the specified iptc value is equal to the current <see cref="IptcValue"/>.</returns>
    public bool Equals(IIptcValue? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Tag != other.Tag)
            return false;

        var data = other.ToByteArray();

        if (_data.Length != data.Length)
            return false;

        for (var i = 0; i < _data.Length; i++)
        {
            if (_data[i] != data[i])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Serves as a hash of this type.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode()
        => _data.GetHashCode() ^ Tag.GetHashCode();

    /// <summary>
    /// Converts this instance to a byte array.
    /// </summary>
    /// <returns>A <see cref="byte"/> array.</returns>
    public byte[] ToByteArray()
    {
        var result = new byte[_data.Length];
        _data.CopyTo(result, 0);
        return result;
    }

    /// <summary>
    /// Returns a string that represents the current value.
    /// </summary>
    /// <returns>A string that represents the current value.</returns>
    public override string ToString()
        => Value;

    private byte[] GetData(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Array.Empty<byte>();
        else
            return _encoding.GetBytes(value);
    }
}
