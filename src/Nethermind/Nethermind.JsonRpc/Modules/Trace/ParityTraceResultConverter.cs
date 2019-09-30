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
using System.Text.Json;
using System.Text.Json.Serialization;
using Nethermind.Core.Json;
using Nethermind.Core.Json.Converters;
using Nethermind.Evm.Tracing;

namespace Nethermind.JsonRpc.Modules.Trace
{
    public class ParityTraceResultConverter : JsonConverter<ParityTraceResult>
    {
        private readonly AddressConverter _addressConverter = new AddressConverter();
        private readonly ByteArrayConverter _byteArrayConverter = new ByteArrayConverter();
        
        public override ParityTraceResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ParityTraceResult value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.Address != null)
            {
                writer.WritePropertyName("address");
                _addressConverter.Write(writer, value.Address, options);
                writer.WritePropertyName("code");
                _byteArrayConverter.Write(writer, value.Code, options);
            }
            
            writer.WriteString("gasUsed", string.Concat("0x", value.GasUsed.ToString("x")));
            
            if(value.Address == null)
            {
                writer.WritePropertyName("output");
                _byteArrayConverter.Write(writer, value.Output, options);
            }
            
            writer.WriteEndObject();
        }
    }
}