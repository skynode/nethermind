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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nethermind.Core;
using Nethermind.Core.Json;
using Nethermind.Core.Json.Converters;
using Nethermind.Evm.Tracing;

namespace Nethermind.JsonRpc.Modules.Trace
{
    public class ParityLikeTxTraceConverter : JsonConverter<ParityLikeTxTrace>
    {
        private readonly ByteArrayConverter _byteArrayConverter = new ByteArrayConverter();
        private readonly KeccakConverter _keccakConverter = new KeccakConverter();

        private readonly ParityAccountStateChangeConverter _accountStateChangeConverter =
            new ParityAccountStateChangeConverter();
        private readonly ParityTraceActionConverter _traceActionConverter = new ParityTraceActionConverter();
        private readonly ParityTraceAddressConverter _traceAddressConverter = new ParityTraceAddressConverter();
        private readonly ParityTraceResultConverter _traceResultConverter = new ParityTraceResultConverter();
        private readonly ParityVmTraceConverter _vmTraceConverter = new ParityVmTraceConverter();

        public override ParityLikeTxTrace Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ParityLikeTxTrace value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("output");
            _byteArrayConverter.Write(writer, value.Output, options);
            writer.WritePropertyName("stateDiff");
            if (value.StateChanges != null)
            {
                writer.WriteStartObject();
                foreach ((Address address, ParityAccountStateChange stateChange) in
                    value.StateChanges.OrderBy(sc => sc.Key, AddressComparer.Instance))
                {
                    writer.WritePropertyName(address.ToString());
                    _accountStateChangeConverter.Write(writer, stateChange, options);
                }

                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNullValue();
            }

            writer.WritePropertyName("trace");
            writer.WriteStartArray();
            if (value.Action != null)
            {
                WriteJson(writer, value.Action, options);
            }
            writer.WriteEndArray();

            if (value.TransactionHash != null)
            {
                writer.WritePropertyName("transactionHash");
                _keccakConverter.Write(writer, value.TransactionHash, options);
            }
            
            writer.WritePropertyName("vmTrace");
            _vmTraceConverter.Write(writer, value.VmTrace, options);

            writer.WriteEndObject();

            /*
             *    "action": {
             *      "callType": "call",
             *      "from": "0x430adc807210dab17ce7538aecd4040979a45137",
             *      "gas": "0x1a1f8",
             *      "input": "0x",
             *      "to": "0x9bcb0733c56b1d8f0c7c4310949e00485cae4e9d",
             *      "value": "0x2707377c7552d8000"
             *    },
             *    "blockHash": "0x3aa472d57e220458fe5b9f1587b9211de68b27504064f5f6e427c68fc1691a29",
             *    "blockNumber": 2392500,
             *    "result": {
             *      "gasUsed": "0x2162",
             *      "output": "0x"
             *    },
             *    "subtraces": 2,
             *    "traceAddress": [],
             *    "transactionHash": "0x847ed5e2e9430bc6ee925a81137ebebe0cea1352209f96723d3503eb7a707aa8",
             *    "transactionPosition": 42,
             *    "type": "call"
             */
        }
        
        private void WriteJson(Utf8JsonWriter writer, ParityTraceAction traceAction, JsonSerializerOptions options)
        {
            if (!traceAction.IncludeInTrace)
            {
                return;
            }
            
            writer.WriteStartObject();
            writer.WritePropertyName("action");
            _traceActionConverter.Write(writer, traceAction, options);
            if (traceAction.Error == null)
            {
                writer.WritePropertyName("result");
                _traceResultConverter.Write(writer, traceAction.Result, options);
            }
            else
            {
                writer.WriteString("error", traceAction.Error);
            }

            writer.WriteNumber("subtraces", traceAction.Subtraces.Count(s => s.IncludeInTrace));
            writer.WritePropertyName("traceAddress");
            _traceAddressConverter.Write(writer, traceAction.TraceAddress, options);

            writer.WriteString("type", traceAction.Type);
            writer.WriteEndObject();
            foreach (ParityTraceAction subtrace in traceAction.Subtraces) WriteJson(writer, subtrace, options);
        }
    }
}