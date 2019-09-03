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

using Nethermind.Db;
using Nethermind.Db.Config;
using Nethermind.Logging;
using Nethermind.Store;

namespace Nethermind.DataMarketplace.Consumers.Infrastructure.Persistence.Rocks
{
    public class ConsumerRocksDbProvider : RocksDbProviderBase, IConsumerDbProvider
    {
        public IDb ConsumerDepositApprovalsDb { get; }
        public IDb ConsumerReceiptsDb { get; }
        public IDb ConsumerSessionsDb { get; }
        public IDb DepositsDb { get; }

        public ConsumerRocksDbProvider(IDbsConfig dbsConfig, ILogManager logManager) : base(dbsConfig, logManager)
        {
            ConsumerDepositApprovalsDb = CreatePartDb(DbParts.ConsumerDepositApprovals);
            ConsumerReceiptsDb = CreatePartDb(DbParts.ConsumerReceipts);
            ConsumerSessionsDb = CreatePartDb(DbParts.ConsumerSessions);
            DepositsDb = CreatePartDb(DbParts.Deposits);
        }

        public override void Dispose()
        {
            ConsumerDepositApprovalsDb?.Dispose();
            ConsumerReceiptsDb?.Dispose();
            ConsumerSessionsDb?.Dispose();
            DepositsDb?.Dispose();
        }
    }
}