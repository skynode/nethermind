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

using Nethermind.Db.Config;

namespace Nethermind.Db
{
    public static class Metrics
    {
        public static long BlocksDbReads => DbParts.Blocks.Reads;
        public static long BlocksDbWrites => DbParts.Blocks.Writes;
        public static long CodeDbReads => DbParts.Code.Reads;
        public static long CodeDbWrites => DbParts.Code.Writes;
        public static long ReceiptsDbReads => DbParts.Receipts.Reads;
        public static long ReceiptsDbWrites => DbParts.Receipts.Writes;
        public static long BlockInfosDbReads => DbParts.BlockInfos.Reads;
        public static long BlockInfosDbWrites => DbParts.BlockInfos.Writes;
        public static long StateDbReads => DbParts.State.Reads;
        public static long StateDbWrites => DbParts.State.Writes;
        public static long PendingTxsDbReads => DbParts.PendingTxs.Reads;
        public static long PendingTxsDbWrites => DbParts.PendingTxs.Writes;
        public static long ConsumerReceiptsDbReads => DbParts.ConsumerReceipts.Reads;
        public static long ConsumerReceiptsDbWrites => DbParts.ConsumerReceipts.Writes;
        public static long ConsumerSessionsDbReads => DbParts.ConsumerSessions.Reads;
        public static long ConsumerSessionsDbWrites => DbParts.ConsumerSessions.Writes;
        public static long ConsumerDepositApprovalsDbReads => DbParts.ConsumerDepositApprovals.Reads;
        public static long ConsumerDepositApprovalsDbWrites => DbParts.ConsumerDepositApprovals.Writes;
        public static long ConfigsDbReads => DbParts.Configs.Reads;
        public static long ConfigsDbWrites => DbParts.Configs.Writes;
        public static long EthRequestsDbReads => DbParts.EthRequests.Reads;
        public static long EthRequestsDbWrites => DbParts.EthRequests.Writes;
        public static long TraceDbReads => DbParts.Trace.Reads;
        public static long TraceDbWrites => DbParts.Trace.Writes;
        public static long HeaderDbReads => DbParts.Headers.Reads;
        public static long HeaderDbWrites  => DbParts.Headers.Writes;
    }
} 