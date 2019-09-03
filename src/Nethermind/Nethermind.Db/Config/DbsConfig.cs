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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Nethermind.Core.Extensions;
using NLog;
using ILogger = Nethermind.Logging.ILogger;

namespace Nethermind.Db.Config
{
    public class DbsConfig : IDbsConfig
    {
        private Dictionary<string, DbConfig> _dbs = new Dictionary<string, DbConfig>();
        private IDictionary<DbParts.DbPart, DbConfig> _parts = GetDefaultConfig();

        private static IDictionary<DbParts.DbPart, DbConfig> GetDefaultConfig() =>
            new Dictionary<DbParts.DbPart, DbConfig>()
            {
                {DbParts.State, new DbConfig(DbParts.State, DbPartConfig.Default16)},
                {DbParts.Trace, new DbConfig(DbParts.Trace, new DbPartConfig(256.MB(), 1024.MB()))},
                {DbParts.Receipts, new DbConfig(DbParts.Receipts, DbPartConfig.Default8)},
                {DbParts.Blocks, new DbConfig(DbParts.Blocks, DbPartConfig.Default8)},
                {DbParts.Headers, new DbConfig(DbParts.Headers, DbPartConfig.Default8)},
                {DbParts.BlockInfos, new DbConfig(DbParts.BlockInfos, new DbPartConfig(8.MB(), 32.MB(), false))},
                {DbParts.PendingTxs, new DbConfig(DbParts.PendingTxs, new DbPartConfig(4.MB(), 16.MB()))},
                {DbParts.Code, new DbConfig(DbParts.Code, new DbPartConfig(2.MB(), 8.MB()))},
                {DbParts.Deposits, new DbConfig(DbParts.Deposits, DbPartConfig.Default16)},
                {DbParts.ConsumerSessions, new DbConfig(DbParts.ConsumerSessions, DbPartConfig.Default16)},
                {DbParts.ConsumerReceipts, new DbConfig(DbParts.ConsumerReceipts, DbPartConfig.Default16)},
                {DbParts.ConsumerDepositApprovals, new DbConfig(DbParts.ConsumerDepositApprovals, DbPartConfig.Default16)},
                {DbParts.Configs, new DbConfig(DbParts.Configs, DbPartConfig.Default16)},
                {DbParts.EthRequests, new DbConfig(DbParts.EthRequests, DbPartConfig.Default16)},
            };

        public IDictionary<DbParts.DbPart, DbConfig> Parts
        {
            get => _parts;
            set
            {
                var currentParts = _parts;
                _parts = value;
                foreach (var part in currentParts)
                {
                    if (!_parts.ContainsKey(part.Key))
                    {
                        _parts.Add(part);
                    }
                }
            }
        }

        public IEnumerable<DbConfig> Dbs 
        {
            get => _dbs.Values;
            set => _dbs = value.ToDictionary(db => db.Key, db => db);
        }
        
        public DbPartConfig Default { get; set; } = DbPartConfig.Default;

        public DbConfig GetPartConfig(DbParts.DbPart dbPart) =>
            dbPart == null
                ? throw new ArgumentException($"Database without specifying part is not configured in {nameof(DbsConfig)}")
                : Parts.TryGetValue(dbPart, out var value)
                    ? value
                    : new DbConfig(dbPart, Default);

        public (DbConfig Config, bool MultiPartDb) GetDbConfig(DbConfig partConfig) => 
            _dbs.TryGetValue(partConfig.Key, out var dbConfig) 
                ? (dbConfig, Parts.Count(p => p.Value.Key == partConfig.Key) > 1) 
                : (partConfig, false);

        public void SetNewDefaultBasePath(string defaultBasePath, ILogger logger)
        {
            SetNewBasePath(defaultBasePath, Default, logger);

            foreach (var db in Dbs)
            {
                SetNewBasePath(defaultBasePath, db.PartConfig);
            }

            foreach (var part in Parts)
            {
                SetNewBasePath(defaultBasePath, part.Value.PartConfig);
            }
        }

        private void SetNewBasePath(string defaultBasePath, DbPartConfig dbPartConfig, ILogger logger = null)
        {
            var currentPath = dbPartConfig.BasePath ?? Default.BasePath ?? ".";
            if (!Path.GetFullPath(currentPath).StartsWith(Path.GetFullPath(defaultBasePath)))
            {
                
                var newDbPath = Path.Combine(defaultBasePath, currentPath);
                if (newDbPath != null)
                {
                    if (logger?.IsDebug == true) logger.Debug($"Adding prefix to baseDbPath, new value: {newDbPath}, old value: {currentPath}");
                    dbPartConfig.BasePath = newDbPath;
                }
                else
                {
                    if (logger?.IsDebug == true) logger.Debug( $"Couldn't add prefix to baseDbPath, prefix: {defaultBasePath}, keeping old value: {currentPath}");
                }
            }
        }
    }
}