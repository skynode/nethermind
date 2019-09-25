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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nethermind.Core.Json;
using Nethermind.Dirichlet.Numerics;
using Nethermind.Evm.Tracing;

namespace Nethermind.JsonRpc.Modules.Trace
{
    public class ParityAccountStateChangeConverter : JsonConverter<ParityAccountStateChange>
    {
        private readonly ByteArrayConverter _bytesConverter = new ByteArrayConverter();
        private readonly UInt256Converter _intConverter = new UInt256Converter();
        private readonly Bytes32Converter _32BytesConverter = new Bytes32Converter();

        public override ParityAccountStateChange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ParityAccountStateChange value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("balance");
            if (value.Balance == null)
            {
                writer.WriteStringValue("=");
            }
            else
            {
                WriteChange(writer, value.Balance, options);
            }

            writer.WritePropertyName("code");
            if (value.Code == null)
            {
                writer.WriteStringValue("=");
            }
            else
            {
                WriteChange(writer, value.Code, options);
            }

            writer.WritePropertyName("nonce");
            if (value.Nonce == null)
            {
                writer.WriteStringValue("=");
            }
            else
            {
                WriteChange(writer, value.Nonce, options);
            }

            writer.WritePropertyName("storage");

            writer.WriteStartObject();
            if (value.Storage != null)
            {
                foreach (KeyValuePair<UInt256, ParityStateChange<byte[]>> pair in value.Storage.OrderBy(s => s.Key))
                {
                    string trimmedKey = pair.Key.ToString("x64");
                    trimmedKey = trimmedKey.Substring(trimmedKey.Length - 64, 64);
                    
                    writer.WritePropertyName(string.Concat("0x", trimmedKey));
                    WriteStorageChange(writer, pair.Value, value.Balance?.Before == null && value.Balance?.After != null, options);
                }
            }

            writer.WriteEndObject();

            writer.WriteEndObject();
        }
        
        private void WriteChange(Utf8JsonWriter writer, ParityStateChange<byte[]> change, JsonSerializerOptions options)
        {
            if (change == null)
            {
                writer.WriteStringValue("=");
            }
            else
            {
                if (change.Before == null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("+");
                    _bytesConverter.Write(writer, change.After, options);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("*");
                    writer.WriteStartObject();
                    writer.WritePropertyName("from");
                    _bytesConverter.Write(writer, change.Before, options);
                    writer.WritePropertyName("to");
                    _bytesConverter.Write(writer, change.After, options);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
            }
        }

        private void WriteChange(Utf8JsonWriter writer, ParityStateChange<UInt256?> change, JsonSerializerOptions options)
        {
            if (change == null)
            {
                writer.WriteStringValue("=");
            }
            else
            {
                if (change.Before == null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("+");
                    _intConverter.Write(writer, change.After ?? 0, options);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("*");
                    writer.WriteStartObject();
                    writer.WritePropertyName("from");
                    _intConverter.Write(writer, change.Before.Value, options);
                    writer.WritePropertyName("to");
                    _intConverter.Write(writer, change.After ?? 0, options);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
            }
        }

        private void WriteStorageChange(Utf8JsonWriter writer, ParityStateChange<byte[]> change, bool isNew, JsonSerializerOptions options)
        {
            if (change == null)
            {
                writer.WriteStringValue("=");
            }
            else
            {
                if (isNew)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("+");
                    _32BytesConverter.Write(writer, change.After, options);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("*");
                    writer.WriteStartObject();
                    writer.WritePropertyName("from");
                    _32BytesConverter.Write(writer, change.Before, options);
                    writer.WritePropertyName("to");
                    _32BytesConverter.Write(writer, change.After, options);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
            }
        }
    }
}