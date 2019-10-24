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
using Nethermind.Evm.Tracing;
using Utf8Json;

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
    public class ParityTraceActionFormatter : IJsonFormatter<ParityTraceAction>
    {
        public void Serialize(ref JsonWriter writer, ParityTraceAction value, IJsonFormatterResolver formatterResolver)
        {
            if (value.Type == "suicide")
            {
                WriteSelfDestructJson(writer, value, formatterResolver);
                return;
            }

            writer.WriteBeginObject();
            if (value.CallType != "create")
            {
                writer.WriteProperty("callType", value.CallType, formatterResolver);
            }

            writer.WriteProperty("from", value.From, formatterResolver);
            writer.WriteProperty("gas", value.Gas, formatterResolver);

            if (value.CallType == "create")
            {
                writer.WriteProperty("init", value.Input, formatterResolver);
            }
            else
            {
                writer.WriteProperty("input", value.Input, formatterResolver);
                writer.WriteProperty("to", value.To, formatterResolver);
            }

            writer.WriteProperty("value", value.Value, formatterResolver, false);
            writer.WriteEndObject();
        }

        private void WriteSelfDestructJson(JsonWriter writer, ParityTraceAction value,
            IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginObject();
            writer.WriteProperty("address", value.From, formatterResolver);
            writer.WriteProperty("balance", value.Value, formatterResolver);
            writer.WriteProperty("refundAddress", value.To, formatterResolver, false);
            writer.WriteEndObject();
        }


        public ParityTraceAction Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }
    }
}