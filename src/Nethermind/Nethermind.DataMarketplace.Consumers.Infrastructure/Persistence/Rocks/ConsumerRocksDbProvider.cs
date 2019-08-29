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
using DbPart = Nethermind.Db.DbParts.DbPart;

namespace Nethermind.DataMarketplace.Consumers.Infrastructure.Persistence.Rocks
{
    public class ConsumerRocksDbProvider : IConsumerDbProvider
    {
        public IDb ConsumerDepositApprovalsDb { get; }
        public IDb ConsumerReceiptsDb { get; }
        public IDb ConsumerSessionsDb { get; }
        public IDb DepositsDb { get; }

        public ConsumerRocksDbProvider(string basePath, IDbsConfig dbsConfig, ILogManager logManager)
        {
            ConsumerDepositApprovalsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.ConsumerDepositApprovals);
            ConsumerReceiptsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.ConsumerReceipts);
            ConsumerSessionsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.ConsumerSessions);
            DepositsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.Deposits);
        }

        private static PartDbOnTheRocks CreatePartDbOnTheRocks(string basePath, IDbsConfig dbsConfig, ILogManager logManager, DbPart dbPart)
        {
            return new PartDbOnTheRocks(basePath, dbPart, dbsConfig[dbPart], logManager);
        }

        public void Dispose()
        {
            ConsumerDepositApprovalsDb?.Dispose();
            ConsumerReceiptsDb?.Dispose();
            ConsumerSessionsDb?.Dispose();
            DepositsDb?.Dispose();
        }
    }
    
    public static class DbParts
    {
        public static readonly DbPart ConsumerDepositApprovals = new DbPart(nameof(ConsumerDepositApprovals));
        public static readonly DbPart ConsumerReceipts = new DbPart(nameof(ConsumerReceipts));
        public static readonly DbPart ConsumerSessions = new DbPart(nameof(ConsumerSessions));
        public static readonly DbPart Deposits = new DbPart(nameof(Deposits));
    }
}