//  Copyright (c) 2018 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.

using System;
using Utf8Json;

namespace Nethermind.Core.Json
{
    internal static class Extensions
    {
        private const int ZeroDigit = 48;
        private const int XChar = 120;

        public static readonly byte[] Bytes0X = {ZeroDigit, XChar};
        public static readonly byte[] Bytes0X0 = {ZeroDigit, XChar, ZeroDigit};

        internal static string ParseSegmentHexString(this ArraySegment<byte> utf8String)
        {
            Span<byte> segment;
            if (utf8String[2] == ZeroDigit)
            {
                segment = utf8String.AsSpan(2);
            }
            else
            {
                segment = utf8String.AsSpan(1);
                segment[0] = ZeroDigit;
            }

            return System.Text.Encoding.UTF8.GetString(segment);
        }

        internal static (bool isString, bool isHex, ArraySegment<byte> segment) GetHexStringSegment(this ref JsonReader reader)
        {
            ArraySegment<byte> segment;
            var isString = false;
            if (reader.GetCurrentJsonToken() == JsonToken.String)
            {
                segment = reader.ReadStringSegmentRaw();
                isString = true;
            }
            else
            {
                segment = reader.ReadNextBlockSegment();
            }

            var isHex = segment.IsHexString();

            return (isString, isHex, segment);
        }
        
        internal static bool IsHexString(this ArraySegment<byte> segment)
            => segment.Count >= 2 && segment[0] == 48 && segment[1] == 120;

        internal static bool IsHexZero(this ArraySegment<byte> utf8String)
            => utf8String.Count == 2 || utf8String.Count == 3 && utf8String[2] == 48;
    }
}