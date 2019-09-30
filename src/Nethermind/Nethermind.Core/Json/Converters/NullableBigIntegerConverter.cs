/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Numerics;
using System.Text.Json;

namespace Nethermind.Core.Json.Converters
{
    public class NullableBigIntegerConverter : System.Text.Json.Serialization.JsonConverter<BigInteger?>
    {
        private readonly BigIntegerConverter _bigIntegerConverter;
        
        public NullableBigIntegerConverter()
            : this(NumberConversion.Hex)
        {
        }

        public NullableBigIntegerConverter(NumberConversion conversion)
        {
            _bigIntegerConverter = new BigIntegerConverter(conversion);
        }

        public override BigInteger? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            
            return _bigIntegerConverter.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, BigInteger? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }
            
            _bigIntegerConverter.Write(writer, value.Value, options);
        }
    }
}