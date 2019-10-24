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
using Nethermind.Core.Extensions;
using Nethermind.JsonRpc.Modules.Trace;
using Utf8Json;

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
    public class ProofFormatter : IJsonFormatter<AccountProof>
    {
        public void Serialize(ref JsonWriter writer, AccountProof value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginObject();
            writer.WritePropertyName("accountProof");
            writer.WriteBeginArray();
            for (int i = 0; i < value.Proof.Length; i++)
            {
                writer.WriteString(value.Proof[i].ToHexString(true));
                if (i < value.Proof.Length - 1)
                {
                    writer.WriteValueSeparator();
                }
            }
            writer.WriteEndArray();
            writer.WriteValueSeparator();
            writer.WriteProperty("balance", value.Balance, formatterResolver);
            writer.WriteProperty("codeHash", value.CodeHash, formatterResolver);
            writer.WriteProperty("nonce", value.Nonce, formatterResolver);
            writer.WriteProperty("storageHash", value.StorageRoot, formatterResolver);
            writer.WritePropertyName("storageProof");
            writer.WriteBeginArray();
            for (int i = 0; i < value.StorageProofs.Length; i++)
            {
                writer.WriteBeginObject();
                writer.WriteProperty("key", value.StorageProofs[i].Key, formatterResolver);
                writer.WritePropertyName("proof");
                writer.WriteBeginArray();
                for (int ip = 0; ip < value.StorageProofs[ip].Proof.Length; ip++)
                {
                    writer.WriteString(value.StorageProofs[i].Proof[ip].ToHexString(true));
                    writer.WriteValueSeparator();
                    if (i < value.StorageProofs[ip].Proof.Length - 1)
                    {
                        writer.WriteValueSeparator();
                    }
                }
                writer.WriteEndArray();
                writer.WriteProperty("value", value.StorageProofs[i].Value, formatterResolver);
                writer.WriteEndObject();
                writer.WriteValueSeparator();
                if (i < value.StorageProofs.Length- 1)
                {
                    writer.WriteValueSeparator();
                }
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public AccountProof Deserialize(ref Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }
    }
}