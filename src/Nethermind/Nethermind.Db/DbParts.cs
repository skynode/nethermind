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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Nethermind.Db
{
    public static class DbParts
    {
        public static readonly DbPart State = nameof(State);
        public static readonly DbPart Trace = nameof(Trace);
        public static readonly DbPart Receipts = nameof(Receipts);
        public static readonly DbPart Blocks = nameof(Blocks);
        public static readonly DbPart Headers = nameof(Headers);
        public static readonly DbPart BlockInfos = nameof(BlockInfos);
        public static readonly DbPart PendingTxs = nameof(PendingTxs);
        public static readonly DbPart Code = nameof(Code);
        public static readonly DbPart Deposits = nameof(Deposits);
        public static readonly DbPart ConsumerSessions = nameof(ConsumerSessions);
        public static readonly DbPart ConsumerReceipts = nameof(ConsumerReceipts);
        public static readonly DbPart ConsumerDepositApprovals = nameof(ConsumerDepositApprovals);
        public static readonly DbPart Configs = nameof(Configs);
        public static readonly DbPart EthRequests = nameof(EthRequests);
        
        public class DbPart : IEquatable<DbPart>
        {
            private static readonly ConcurrentDictionary<string, DbPart> DbParts = new ConcurrentDictionary<string, DbPart>();
            
            public string Name { get; }
            public int Reads { get; set; }
            public int Writes { get; set; }

            private DbPart(string name)
            {
                Name = name;
            }

            public static DbPart Get(string name) => DbParts.GetOrAdd(name, new DbPart(name));

            public bool Equals(DbPart other) => Name == other?.Name;

            public override bool Equals(object obj) => obj is DbPart other && Equals(other);

            public override int GetHashCode() => Name.GetHashCode();

            public override string ToString() => Name;
            
            public static implicit operator DbPart(string value) => Get(value);
        }
    }
}