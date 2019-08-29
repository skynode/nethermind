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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nethermind.Core.Extensions;

namespace Nethermind.Db.Config
{
    public class SeparateDbsConfig : IDbsConfig
    {
        private readonly IDictionary<DbParts.DbPart, IDbConfig> _dictionary;
        
        public static SeparateDbsConfig Default = new SeparateDbsConfig();

        public SeparateDbsConfig(IDictionary<DbParts.DbPart, IDbConfig> dictionary)
        {
            _dictionary = dictionary;
        }
        
        public SeparateDbsConfig() : this(GetDefaultConfig())
        {
        }

        private static IDictionary<DbParts.DbPart, IDbConfig> GetDefaultConfig() =>
            new Dictionary<DbParts.DbPart, IDbConfig>()
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

        public IDbConfig GetPartConfig(DbParts.DbPart dbPart) =>
            dbPart == null
                ? throw new ArgumentException($"Database without specifying part is not configured in {nameof(SeparateDbsConfig)}")
                : _dictionary.TryGetValue(dbPart, out var value)
                    ? value
                    : new DbConfig(dbPart, DbPartConfig.Default);
    }
}