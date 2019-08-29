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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Nethermind.Core.Extensions;
using Nethermind.Db.Config;
using Nethermind.Logging;
using RocksDbSharp;

namespace Nethermind.Db
{
    public abstract class DbOnTheRocks
    {
        protected readonly RocksDb Db;
        private WriteBatch _currentBatch;
        
        public string Name { get; }

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected DbOnTheRocks(string basePath, string name, IDbPartConfig dbConfig, ILogManager logManager = null)
        {
            Name = name;
            var fullPath = Path.Combine(basePath, Name.ToCamelCase());
            Directory.CreateDirectory(fullPath);
            var logger = logManager?.GetClassLogger();
            if (logger != null)
            {
                if (logger.IsInfo) logger.Info($"Using database directory {fullPath}");
            }
            
            Db = CreateDb(dbConfig, fullPath);
        }

        private RocksDb CreateDb(IDbPartConfig dbConfig, string fullPath)
        {
            try
            {
                var options = BuildOptions(dbConfig);
                return OpenDb(options, fullPath);
            }
            catch (DllNotFoundException e) when (e.Message.Contains("libdl"))
            {
                throw new ApplicationException(
                    $"Unable to load 'libdl' necessary to init the RocksDB database. Please run{Environment.NewLine}" +
                    "sudo apt update && sudo apt install libsnappy-dev libc6-dev libc6");
            }
        }

        protected abstract RocksDb OpenDb(DbOptions options, string fullPath);
        
        protected abstract DbParts.DbPart GetDbPart(ColumnFamilyHandle cf);
        
        protected virtual DbOptions BuildOptions(IDbPartConfig dbConfig)
        {
            var tableOptions = new BlockBasedTableOptions();
            tableOptions.SetBlockSize(16.KB());
            tableOptions.SetPinL0FilterAndIndexBlocksInCache(true);
            tableOptions.SetCacheIndexAndFilterBlocks(dbConfig.CacheIndexAndFilterBlocks);

            tableOptions.SetFilterPolicy(BloomFilterPolicy.Create(10, true));
            tableOptions.SetFormatVersion(2);

            var blockCacheSize = dbConfig.BlockCacheSize;
            var cache = Native.Instance.rocksdb_cache_create_lru(new UIntPtr(blockCacheSize));
            tableOptions.SetBlockCache(cache);

            var options = new DbOptions();
            options.SetCreateIfMissing(true);
            options.SetAdviseRandomOnOpen(true);
            options.OptimizeForPointLookup(blockCacheSize); // I guess this should be the one option controlled by the DB size property - bind it to LRU cache size
            // options.SetCompression(CompressionTypeEnum.rocksdb_snappy_compression);
            // options.SetLevelCompactionDynamicLevelBytes(true);

            /*
             * Multi-Threaded Compactions
             * Compactions are needed to remove multiple copies of the same key that may occur if an application overwrites an existing key. Compactions also process deletions of keys. Compactions may occur in multiple threads if configured appropriately.
             * The entire database is stored in a set of sstfiles. When a memtable is full, its content is written out to a file in Level-0 (L0). RocksDB removes duplicate and overwritten keys in the memtable when it is flushed to a file in L0. Some files are periodically read in and merged to form larger files - this is called compaction.
             * The overall write throughput of an LSM database directly depends on the speed at which compactions can occur, especially when the data is stored in fast storage like SSD or RAM. RocksDB may be configured to issue concurrent compaction requests from multiple threads. It is observed that sustained write rates may increase by as much as a factor of 10 with multi-threaded compaction when the database is on SSDs, as compared to single-threaded compactions.
             * TKS: Observed 500MB/s compared to ~100MB/s between multithreaded and single thread compactions on my machine (processor count is returning 12 for 6 cores with hyperthreading)
             * TKS: CPU goes to insane 30% usage on idle - compacting only app
             */
            options.SetMaxBackgroundCompactions(Environment.ProcessorCount);

            // options.SetMaxOpenFiles(32);
            options.SetWriteBufferSize(dbConfig.WriteBufferSize);
            options.SetMaxWriteBufferNumber((int)dbConfig.WriteBufferNumber);
            options.SetMinWriteBufferNumberToMerge(2);
            options.SetBlockBasedTableFactory(tableOptions);
            
            options.SetMaxBackgroundFlushes(Environment.ProcessorCount);
            options.IncreaseParallelism(Environment.ProcessorCount);
            // options.SetLevelCompactionDynamicLevelBytes(true); // only switch on on empty DBs
            return options;
        }

        private void UpdateReadMetrics(ColumnFamilyHandle cf)
        {
            GetDbPart(cf).Reads++;
        }

        private void UpdateWriteMetrics(ColumnFamilyHandle cf)
        {
            GetDbPart(cf).Writes++;
        }

        public byte[] this[byte[] key, ColumnFamilyHandle cf = null]
        {
            get
            {
                UpdateReadMetrics(cf);
                return Db.Get(key, cf);
            }
            set
            {
                UpdateWriteMetrics(cf);
                if (_currentBatch != null)
                {
                    if (value == null)
                    {
                        _currentBatch.Delete(key, cf);
                    }
                    else
                    {
                        _currentBatch.Put(key, value, cf);
                    }
                }
                else
                {
                    if (value == null)
                    {
                        Db.Remove(key, cf);
                    }
                    else
                    {
                        Db.Put(key, value, cf);
                    }
                }
            }
        }

        public Span<byte> GetSpan(byte[] key, ColumnFamilyHandle cf = null)
        {
            UpdateReadMetrics(cf);
            return Db.GetSpan(key, cf);
        }

        public void DangerousReleaseMemory(in Span<byte> span)
        {
            Db.DangerousReleaseMemory(in span);
        }

        public void Remove(byte[] key, ColumnFamilyHandle cf = null)
        {
            Db.Remove(key, cf);
        }

        public byte[][] GetAll(ColumnFamilyHandle cf = null)
        {
            var iterator = Db.NewIterator(cf);
            using (iterator = iterator.SeekToFirst())
            {
                var values = new List<byte[]>();
                while (iterator.Valid())
                {
                    values.Add(iterator.Value());
                    iterator = iterator.Next();
                }
                
                return values.ToArray();
            }
        }

        private byte[] _keyExistsBuffer = new byte[1];
        
        public bool KeyExists(byte[] key, ColumnFamilyHandle cf = null)
        {
            // seems it has no performance impact
            return Db.Get(key, cf) != null;
//            return _db.Get(key, 32, _keyExistsBuffer, 0, 0, null, null) != -1;
        }
        
        public void StartBatch()
        {
            _currentBatch = new WriteBatch();
        }

        public void CommitBatch()
        {
            Db.Write(_currentBatch);
            _currentBatch.Dispose();
            _currentBatch = null;
        }

        public void Dispose()
        {
            Db?.Dispose();
            _currentBatch?.Dispose();
        }
    }
}