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
using Nethermind.Logging;
using Nethermind.Store;

namespace Nethermind.Db
{
    public class RocksDbProvider : IDbProvider
    {
        public RocksDbProvider(string basePath, IDbsConfig dbsConfig, ILogManager logManager, bool useTraceDb, bool useReceiptsDb)
        {
            BlocksDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.Blocks);
            HeadersDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.Headers);
            BlockInfosDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.BlockInfos);
            StateDb = new StateDb(CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.State));
            CodeDb = new StateDb(CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.Code));
            PendingTxsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.PendingTxs);
            ConfigsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.Configs);
            EthRequestsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.EthRequests);
            
            if (useReceiptsDb)
            {
                ReceiptsDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.Receipts);
            }
            else
            {
                ReceiptsDb = new ReadOnlyDb(new MemDb(), false);
            }
            
            if (useTraceDb)
            {
                TraceDb = CreatePartDbOnTheRocks(basePath, dbsConfig, logManager, DbParts.Trace);
            }
            else
            {
                TraceDb = new ReadOnlyDb(new MemDb(), false);
            }
        }
        
        private static IDb CreatePartDbOnTheRocks(string basePath, IDbsConfig dbsConfig, ILogManager logManager, DbParts.DbPart dbPart)
        {
            return new PartDbOnTheRocks(basePath, dbPart, dbsConfig[dbPart], logManager);
        }
        
        public ISnapshotableDb StateDb { get; }
        public ISnapshotableDb CodeDb { get; }
        public IDb TraceDb { get; }
        public IDb ReceiptsDb { get; }
        public IDb BlocksDb { get; }
        public IDb HeadersDb { get; }
        public IDb BlockInfosDb { get; }
        public IDb PendingTxsDb { get; }
        public IDb ConfigsDb { get; }
        public IDb EthRequestsDb { get; }

        public void Dispose()
        {
            StateDb?.Dispose();
            CodeDb?.Dispose();
            ReceiptsDb?.Dispose();
            BlocksDb?.Dispose();
            HeadersDb?.Dispose();
            BlockInfosDb?.Dispose();
            PendingTxsDb?.Dispose();
            TraceDb?.Dispose();
            ConfigsDb?.Dispose();
            EthRequestsDb?.Dispose();
        }
    }
    
        public class ColumnRocksDbProvider : IDbProvider
        {
            public ColumnRocksDbProvider(string basePath, IDbsConfig dbsConfig, ILogManager logManager, bool useTraceDb, bool useReceiptsDb)
            {
//                BlocksDb = new BlocksRocksDb(basePath, dbConfig, logManager);
//                HeadersDb = new HeadersRocksDb(basePath, dbConfig, logManager);
//                BlockInfosDb = new BlockInfosRocksDb(basePath, dbConfig, logManager);
//                StateDb = new StateDb(new StateRocksDb(basePath, dbConfig, logManager));
//                CodeDb = new StateDb(new CodeRocksDb(basePath, dbConfig, logManager));
//                PendingTxsDb = new PendingTxsRocksDb(basePath, dbConfig, logManager);
//                ConfigsDb = new ConfigsRocksDb(basePath, dbConfig, logManager);
//                EthRequestsDb = new EthRequestsRocksDb(basePath, dbConfig, logManager);
//                
//                if (useReceiptsDb)
//                {
//                    ReceiptsDb = new ReceiptsRocksDb(basePath, dbConfig, logManager);
//                }
//                else
//                {
//                    ReceiptsDb = new ReadOnlyDb(new MemDb(), false);
//                }
//                
//                TraceDb = new TraceRocksDb(basePath, dbConfig, logManager);
//                
//                if (useTraceDb)
//                {
//                    TraceDb = new TraceRocksDb(basePath, dbConfig, logManager);
//                }
//                else
//                {
//                    TraceDb = new ReadOnlyDb(new MemDb(), false);
//                }
            }
        
        public ISnapshotableDb StateDb { get; }
        public ISnapshotableDb CodeDb { get; }
        public IDb TraceDb { get; }
        public IDb ReceiptsDb { get; }
        public IDb BlocksDb { get; }
        public IDb HeadersDb { get; }
        public IDb BlockInfosDb { get; }
        public IDb PendingTxsDb { get; }
        public IDb ConfigsDb { get; }
        public IDb EthRequestsDb { get; }

        public void Dispose()
        {
            StateDb?.Dispose();
            CodeDb?.Dispose();
            ReceiptsDb?.Dispose();
            BlocksDb?.Dispose();
            HeadersDb?.Dispose();
            BlockInfosDb?.Dispose();
            PendingTxsDb?.Dispose();
            TraceDb?.Dispose();
            ConfigsDb?.Dispose();
            EthRequestsDb?.Dispose();
        }
    }
}