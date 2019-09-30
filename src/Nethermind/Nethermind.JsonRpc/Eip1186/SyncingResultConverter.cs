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
using Nethermind.Core.Extensions;
using Nethermind.Core.Json;
using Nethermind.Core.Json.Converters;

namespace Nethermind.JsonRpc.Eip1186
{
    /// <summary>
    ///{
    ///  "id": 1,
    ///  "jsonrpc": "2.0",
    ///  "method": "eth_getProof",
    ///  "params": [
    ///    "0x7F0d15C7FAae65896648C8273B6d7E43f58Fa842",
    ///    [  "0x56e81f171bcc55a6ff8345e692c0f86e5b48e01b996cadc001622fb5e363b421" ],
    ///    "latest"
    ///  ]
    ///}
    ///  
    ///{
    ///  "id": 1,
    ///  "jsonrpc": "2.0",
    ///  "result": {
    ///    "accountProof": [
    ///    "0xf90211a...0701bc80",
    ///    "0xf90211a...0d832380",
    ///    "0xf90211a...5fb20c80",
    ///    "0xf90211a...0675b80",
    ///    "0xf90151a0...ca08080"
    ///    ],
    ///  "balance": "0x0",
    ///  "codeHash": "0xc5d2460186f7233c927e7db2dcc703c0e500b653ca82273b7bfad8045d85a470",
    ///  "nonce": "0x0",
    ///  "storageHash": "0x56e81f171bcc55a6ff8345e692c0f86e5b48e01b996cadc001622fb5e363b421",
    ///  "storageProof": [
    ///  {
    ///    "key": "0x56e81f171bcc55a6ff8345e692c0f86e5b48e01b996cadc001622fb5e363b421",
    ///    "proof": [
    ///    "0xf90211a...0701bc80",
    ///    "0xf90211a...0d832380"
    ///    ],
    ///    "value": "0x1"
    ///  }
    ///  ]
    ///  }
    ///}
    /// </summary>
    public class ProofConverter : JsonConverter<AccountProof>
    {
        private readonly KeccakConverter _keccakConverter = new KeccakConverter();
        private readonly UInt256Converter _uInt256Converter = new UInt256Converter();
        
        public override AccountProof Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, AccountProof value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("accountProof");
            writer.WriteStartArray();
            for (int i = 0; i < value.Proof.Length; i++)
            {
                writer.WriteStringValue(value.Proof[i].ToHexString(true));    
            }
            writer.WriteEndArray();
            writer.WritePropertyName("balance");
            _uInt256Converter.Write(writer, value.Balance, options);
            writer.WritePropertyName("codeHash");
            _keccakConverter.Write(writer, value.CodeHash, options);
            writer.WritePropertyName("nonce");
            _uInt256Converter.Write(writer, value.Nonce, options);
            writer.WritePropertyName("storageHash");
            _keccakConverter.Write(writer, value.StorageRoot, options);
            writer.WritePropertyName("storageProof");
            writer.WriteStartArray();
            for (int i = 0; i < value.StorageProofs.Length; i++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("key");
                _keccakConverter.Write(writer, value.StorageProofs[i].Key, options);
                writer.WritePropertyName("proof");
                writer.WriteStartArray();
                for (int ip = 0; ip < value.StorageProofs[ip].Proof.Length; ip++)
                {
                    writer.WriteStringValue(value.StorageProofs[i].Proof[ip].ToHexString(true));    
                }
                writer.WriteEndArray();
                writer.WriteString("value", value.StorageProofs[i].Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}