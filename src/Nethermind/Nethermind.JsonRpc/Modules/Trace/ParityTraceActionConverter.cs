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
    /*
     * {
     *   "callType": "call",
     *   "from": "0x430adc807210dab17ce7538aecd4040979a45137",
     *   "gas": "0x1a1f8",
     *   "input": "0x",
     *   "to": "0x9bcb0733c56b1d8f0c7c4310949e00485cae4e9d",
     *    "value": "0x2707377c7552d8000"
     * },
     */
    public class ParityTraceActionConverter : JsonConverter<ParityTraceAction>
    {
        private readonly AddressConverter _addressConverter = new AddressConverter();
        private readonly ByteArrayConverter _byteArrayConverter = new ByteArrayConverter();
        private readonly UInt256Converter _uInt256Converter = new UInt256Converter();
        
        public override ParityTraceAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ParityTraceAction value, JsonSerializerOptions options)
        {
            if (value.Type == "suicide")
            {
                WriteSelfDestructJson(writer, value, options);
                return;
            }
            
            writer.WriteStartObject();
            if (value.CallType != "create")
            {
                writer.WriteString("callType", value.CallType);
            }
 
            writer.WritePropertyName("from");
            _addressConverter.Write(writer, value.From, options);
            writer.WriteNumber("gas", value.Gas);

            if (value.CallType == "create")
            {
                writer.WritePropertyName("init");
                _byteArrayConverter.Write(writer, value.Input, options);
            }
            else
            {
                
                writer.WritePropertyName("init");
                _byteArrayConverter.Write(writer, value.Input, options);
                writer.WritePropertyName("to");
                _addressConverter.Write(writer, value.To, options);    
            }
            
            writer.WritePropertyName("value");
            _uInt256Converter.Write(writer, value.Value, options);
            writer.WriteEndObject();
        }
        
        private void WriteSelfDestructJson(Utf8JsonWriter writer, ParityTraceAction value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("address");
            _addressConverter.Write(writer, value.From, options);
            writer.WritePropertyName("balance");
            _uInt256Converter.Write(writer, value.Value, options);
            writer.WritePropertyName("refundAddress");
            _addressConverter.Write(writer, value.To, options);
            writer.WriteEndObject();
        }
        
    }
}