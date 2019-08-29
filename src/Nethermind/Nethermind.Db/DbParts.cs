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
using System.Threading;

namespace Nethermind.Db
{
    public static class DbParts
    {
        public static readonly DbPart State = new DbPart(nameof(State));
        public static readonly DbPart Trace = new DbPart(nameof(Trace));
        public static readonly DbPart Receipts = new DbPart(nameof(Receipts));
        public static readonly DbPart Blocks = new DbPart(nameof(Blocks));
        public static readonly DbPart Headers = new DbPart(nameof(Headers));
        public static readonly DbPart BlockInfos = new DbPart(nameof(BlockInfos));
        public static readonly DbPart PendingTxs = new DbPart(nameof(PendingTxs));
        public static readonly DbPart Code = new DbPart(nameof(Code));
        public static readonly DbPart Deposits = new DbPart(nameof(Deposits));
        public static readonly DbPart ConsumerSessions = new DbPart(nameof(ConsumerSessions));
        public static readonly DbPart ConsumerReceipts = new DbPart(nameof(ConsumerReceipts));
        public static readonly DbPart ConsumerDepositApprovals = new DbPart(nameof(ConsumerDepositApprovals));
        public static readonly DbPart Configs = new DbPart(nameof(Configs));
        public static readonly DbPart EthRequests = new DbPart(nameof(EthRequests));
        
        public class DbPart : IEquatable<DbPart>
        {
            private static int _maxValue = -1;
            
            public int Value { get; }
            public string Name { get; }
            public int Reads { get; set; }
            public int Writes { get; set; }

            public DbPart(string name)
            {
                Value = GetValue();
                Name = name;
            }

            private static int GetValue() => Interlocked.Increment(ref _maxValue);

            public bool Equals(DbPart other) => Value == other?.Value;

            public override bool Equals(object obj) => obj is DbPart other && Equals(other);

            public override int GetHashCode() => Value;

            public override string ToString() => Name;
        }
    }
}