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
using Nethermind.Core.Json;
using Nethermind.Dirichlet.Numerics;
using Nethermind.Evm.Tracing;
using Utf8Json;

namespace Nethermind.JsonRpc.Modules.Trace
{
    public class ParityAccountStateChangeFormatter : IJsonFormatter<ParityAccountStateChange>
    {
        private readonly ByteArrayFormatter _bytesFormatter = new ByteArrayFormatter();
        private readonly NullableUInt256Formatter _intFormatter = new NullableUInt256Formatter();
        private readonly Bytes32Formatter _32BytesFormatter = new Bytes32Formatter();

        public void Serialize(ref JsonWriter writer, ParityAccountStateChange value,
            IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginObject();
            writer.WritePropertyName("balance");
            if (value.Balance == null)
            {
                writer.WriteString("=");
                writer.WriteValueSeparator();
            }
            else
            {
                WriteChange(ref writer, value.Balance, formatterResolver);
                writer.WriteValueSeparator();
            }

            writer.WritePropertyName("code");
            if (value.Code == null)
            {
                writer.WriteString("=");
                writer.WriteValueSeparator();
            }
            else
            {
                WriteChange(ref writer, value.Code, formatterResolver);
                writer.WriteValueSeparator();
            }

            writer.WritePropertyName("nonce");
            if (value.Nonce == null)
            {
                writer.WriteString("=");
                writer.WriteValueSeparator();
            }
            else
            {
                WriteChange(ref writer, value.Nonce, formatterResolver);
                writer.WriteValueSeparator();
            }

            writer.WritePropertyName("storage");

            writer.WriteBeginObject();
            if (value.Storage != null)
            {
                foreach (KeyValuePair<UInt256, ParityStateChange<byte[]>> pair in value.Storage.OrderBy(s => s.Key))
                {
                    string trimmedKey = pair.Key.ToString("x64");
                    trimmedKey = trimmedKey.Substring(trimmedKey.Length - 64, 64);

                    writer.WritePropertyName(string.Concat("0x", trimmedKey));
                    WriteStorageChange(ref writer, pair.Value,
                        value.Balance?.Before == null && value.Balance?.After != null, formatterResolver);
                }
            }

            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        private void WriteChange(ref JsonWriter writer, ParityStateChange<byte[]> change,
            IJsonFormatterResolver formatterResolver)
        {
            if (change == null)
            {
                writer.WriteString("=");
            }
            else
            {
                if (change.Before == null)
                {
                    writer.WriteBeginObject();
                    writer.WritePropertyName("+");
                    _bytesFormatter.Serialize(ref writer, change.After, formatterResolver);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteBeginObject();
                    writer.WritePropertyName("*");
                    writer.WriteBeginObject();
                    writer.WritePropertyName("from");
                    _bytesFormatter.Serialize(ref writer, change.Before, formatterResolver);
                    writer.WriteValueSeparator();
                    writer.WritePropertyName("to");
                    _bytesFormatter.Serialize(ref writer, change.After, formatterResolver);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
            }
        }

        private void WriteChange(ref JsonWriter writer, ParityStateChange<UInt256?> change,
            IJsonFormatterResolver formatterResolver)
        {
            if (change == null)
            {
                writer.WriteString("=");
            }
            else
            {
                if (change.Before == null)
                {
                    writer.WriteBeginObject();
                    writer.WritePropertyName("+");
                    _intFormatter.Serialize(ref writer, change.After, formatterResolver);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteBeginObject();
                    writer.WritePropertyName("*");
                    writer.WriteBeginObject();
                    writer.WritePropertyName("from");
                    _intFormatter.Serialize(ref writer, change.Before, formatterResolver);
                    writer.WriteValueSeparator();
                    writer.WritePropertyName("to");
                    _intFormatter.Serialize(ref writer, change.After, formatterResolver);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
            }
        }

        private void WriteStorageChange(ref JsonWriter writer, ParityStateChange<byte[]> change, bool isNew,
            IJsonFormatterResolver formatterResolver)
        {
            if (change == null)
            {
                writer.WriteString("=");
            }
            else
            {
                if (isNew)
                {
                    writer.WriteBeginObject();
                    writer.WritePropertyName("+");
                    _32BytesFormatter.Serialize(ref writer, change.After, formatterResolver);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteBeginObject();
                    writer.WritePropertyName("*");
                    writer.WriteBeginObject();
                    writer.WritePropertyName("from");
                    _32BytesFormatter.Serialize(ref writer, change.Before, formatterResolver);
                    writer.WriteValueSeparator();
                    writer.WritePropertyName("to");
                    _32BytesFormatter.Serialize(ref writer, change.After, formatterResolver);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
            }
        }

        public ParityAccountStateChange Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }
    }
}