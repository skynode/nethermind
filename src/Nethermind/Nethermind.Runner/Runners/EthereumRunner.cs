﻿/*
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
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethermind.Blockchain;
using Nethermind.Blockchain.Difficulty;
using Nethermind.Blockchain.Validators;
using Nethermind.Config;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Model;
using Nethermind.Core.Specs;
using Nethermind.Core.Specs.ChainSpec;
using Nethermind.Db;
using Nethermind.Evm;
using Nethermind.KeyStore;
using Nethermind.Network;
using Nethermind.Network.Crypto;
using Nethermind.Network.Discovery;
using Nethermind.Network.Discovery.Lifecycle;
using Nethermind.Network.Discovery.Messages;
using Nethermind.Network.Discovery.RoutingTable;
using Nethermind.Network.Discovery.Serializers;
using Nethermind.Network.P2P;
using Nethermind.Network.P2P.Subprotocols.Eth;
using Nethermind.Network.Rlpx;
using Nethermind.Network.Rlpx.Handshake;
using Nethermind.Network.Stats;
using Nethermind.Store;
using PingMessageSerializer = Nethermind.Network.P2P.PingMessageSerializer;
using PongMessageSerializer = Nethermind.Network.P2P.PongMessageSerializer;

namespace Nethermind.Runner.Runners
{
    public class EthereumRunner : IEthereumRunner
    {
        private static NLogLogger _defaultLogger = new NLogLogger("default");
        private static NLogLogger _evmLogger = new NLogLogger("evm");
        private static NLogLogger _stateLogger = new NLogLogger("state");
        private static NLogLogger _chainLogger = new NLogLogger("chain");
        private static NLogLogger _networkLogger = new NLogLogger("net");
        private static NLogLogger _discoveryLogger = new NLogLogger("discovery");

        private static string _dbBasePath;
        private readonly IConfigProvider _configProvider;
        private readonly INetworkHelper _networkHelper;
        private IBlockchainProcessor _blockchainProcessor;
        private ICryptoRandom _cryptoRandom;
        private IDiscoveryApp _discoveryApp;
        private IDiscoveryManager _discoveryManager;
        private IRlpxPeer _localPeer;
        private IMessageSerializationService _messageSerializationService;
        private INodeFactory _nodeFactory;
        private INodeStatsProvider _nodeStatsProvider;
        private IPerfService _perfService;
        private PrivateKey _privateKey;
        private CancellationTokenSource _runnerCancellation;
        private ISigner _signer;
        private ISynchronizationManager _syncManager;
        private ITransactionTracer _tracer;

        public EthereumRunner(IConfigProvider configurationProvider, INetworkHelper networkHelper)
        {
            _configProvider = configurationProvider;
            _networkHelper = networkHelper;
        }

        public async Task Start(InitParams initParams)
        {
            ConfigureTools(initParams);
            await InitBlockchain(initParams);
            _defaultLogger.Info("Ethereum initialization completed");
        }

        private void ConfigureTools(InitParams initParams)
        {
            _runnerCancellation = new CancellationTokenSource();
            _defaultLogger = new NLogLogger(initParams.LogFileName, "default");
            _evmLogger = new NLogLogger(initParams.LogFileName, "evm");
            _stateLogger = new NLogLogger(initParams.LogFileName, "state");
            _chainLogger = new NLogLogger(initParams.LogFileName, "chain");
            _networkLogger = new NLogLogger(initParams.LogFileName, "net");
            _discoveryLogger = new NLogLogger(initParams.LogFileName, "discovery");

            _defaultLogger.LogLoggerInfo();
            _evmLogger.LogLoggerInfo();
            _stateLogger.LogLoggerInfo();
            _chainLogger.LogLoggerInfo();
            _networkLogger.LogLoggerInfo();
            _discoveryLogger.LogLoggerInfo();

            _defaultLogger.Info("Initializing Ethereum");
            _defaultLogger.Info($"Server GC           : {System.Runtime.GCSettings.IsServerGC}");
            _defaultLogger.Info($"GC latency mode     : {System.Runtime.GCSettings.LatencyMode}");
            _defaultLogger.Info($"LOH compaction mode : {System.Runtime.GCSettings.LargeObjectHeapCompactionMode}");
            _privateKey = new PrivateKey(initParams.TestNodeKey);
            _dbBasePath = initParams.BaseDbPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db");

            _tracer = initParams.TransactionTracingEnabled ? new TransactionTracer(initParams.BaseTracingPath, new UnforgivingJsonSerializer()) : NullTracer.Instance;
            _perfService = new PerfService(_defaultLogger);
        }

        public async Task StopAsync()
        {
            _networkLogger.Info("Shutting down...");
            _runnerCancellation.Cancel();
            _networkLogger.Info("Stopping sync manager...");
            await (_syncManager?.StopAsync() ?? Task.CompletedTask);
            _networkLogger.Info("Stopping blockchain processor...");
            await (_blockchainProcessor?.StopAsync() ?? Task.CompletedTask);
            _networkLogger.Info("Stopping local peer...");
            await (_localPeer?.Shutdown() ?? Task.CompletedTask);
            _networkLogger.Info("Goodbye...");
        }

        private ChainSpec LoadChainSpec(string chainSpecFile)
        {
            _defaultLogger.Info($"Loading ChainSpec from {chainSpecFile}");
            ChainSpecLoader loader = new ChainSpecLoader(new UnforgivingJsonSerializer());
            if (!Path.IsPathRooted(chainSpecFile))
            {
                chainSpecFile = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, chainSpecFile));
            }

            ChainSpec chainSpec = loader.Load(File.ReadAllBytes(chainSpecFile));
            return chainSpec;
        }

        private async Task InitBlockchain(InitParams initParams)
        {
            ChainSpec chainSpec = LoadChainSpec(initParams.ChainSpecPath);
            
            /* spec */
            // TODO: rebuild to use chainspec
            ISpecProvider specProvider;
            if (chainSpec.ChainId == RopstenSpecProvider.Instance.ChainId)
            {
                specProvider = RopstenSpecProvider.Instance;
            }
            else if (chainSpec.ChainId == MainNetSpecProvider.Instance.ChainId)
            {
                specProvider = MainNetSpecProvider.Instance;
            }
            else
            {
                throw new NotSupportedException($"Not yet tested, not yet supported ChainId {chainSpec.ChainId}");
            }
            
            var ethereumSigner = new EthereumSigner(specProvider, _chainLogger);
            var transactionStore = new TransactionStore();
            var sealEngine = ConfigureSealEngine(transactionStore, ethereumSigner, initParams);

            /* sync */
            var blocksDb = new DbOnTheRocks(Path.Combine(_dbBasePath, DbOnTheRocks.BlocksDbPath));
            var blockInfosDb = new DbOnTheRocks(Path.Combine(_dbBasePath, DbOnTheRocks.BlockInfosDbPath));
            var receiptsDb = new DbOnTheRocks(Path.Combine(_dbBasePath, DbOnTheRocks.ReceiptsDbPath));

            /* blockchain */
            var blockTree = new BlockTree(blocksDb, blockInfosDb, receiptsDb, specProvider, _chainLogger);
            var difficultyCalculator = new DifficultyCalculator(specProvider);
            
            /* validation */
            var headerValidator = new HeaderValidator(difficultyCalculator, blockTree, sealEngine, specProvider, _chainLogger);
            var ommersValidator = new OmmersValidator(blockTree, headerValidator, _chainLogger);
            var txValidator = new TransactionValidator(new SignatureValidator(specProvider.ChainId));
            var blockValidator = new BlockValidator(txValidator, headerValidator, ommersValidator, specProvider, _chainLogger);

            /* state */
            var dbProvider = new RocksDbProvider(_dbBasePath, _stateLogger);
            var codeDb = new DbOnTheRocks(Path.Combine(_dbBasePath, DbOnTheRocks.CodeDbPath));
            var stateDb = new DbOnTheRocks(Path.Combine(_dbBasePath, DbOnTheRocks.StateDbPath));
            var stateTree = new StateTree(stateDb);
            var stateProvider = new StateProvider(stateTree, _stateLogger, codeDb);
            var storageProvider = new StorageProvider(dbProvider, stateProvider, _stateLogger);

            /* blockchain processing */
            var blockhashProvider = new BlockhashProvider(blockTree);
            var virtualMachine = new VirtualMachine(stateProvider, storageProvider, blockhashProvider, _evmLogger);
            var transactionProcessor = new TransactionProcessor(specProvider, stateProvider, storageProvider, virtualMachine, _tracer, _chainLogger);
            var rewardCalculator = new RewardCalculator(specProvider);
            var blockProcessor = new BlockProcessor(specProvider, blockValidator, rewardCalculator, transactionProcessor, dbProvider, stateProvider, storageProvider, transactionStore, _chainLogger);
            _blockchainProcessor = new BlockchainProcessor(blockTree, sealEngine, transactionStore, difficultyCalculator, blockProcessor, ethereumSigner, _chainLogger);
            _blockchainProcessor.Start();

            LoadGenesisBlock(chainSpec, string.IsNullOrWhiteSpace(initParams.GenesisHash) ? null : new Keccak(initParams.GenesisHash), blockTree, stateProvider, specProvider);
            await StartProcessing(blockTree, transactionStore, blockValidator, headerValidator, txValidator, initParams);
        }

        private async Task StartProcessing(BlockTree blockTree, TransactionStore transactionStore, BlockValidator blockValidator, HeaderValidator headerValidator, TransactionValidator txValidator, InitParams initParams)
        {
            await blockTree.LoadBlocksFromDb(_runnerCancellation.Token).ContinueWith(async t =>
            {
                if (t.IsFaulted)
                {
                    if (_chainLogger.IsErrorEnabled) _chainLogger.Error("Loading blocks from DB failed.", t.Exception);
                }
                else if (t.IsCanceled)
                {
                    if (_chainLogger.IsWarnEnabled) _chainLogger.Warn("Loading blocks from DB cancelled.");
                }
                else
                {
                    if (_chainLogger.IsInfoEnabled) _chainLogger.Info("Loaded all blocks from DB");
                    if (!initParams.SynchronizationEnabled)
                    {
                        if (_chainLogger.IsInfoEnabled) _chainLogger.Info("Synchronization disabled.");    
                    }
                    else
                    {
                        if (_chainLogger.IsInfoEnabled) _chainLogger.Info("Starting synchronization.");
                        // TODO: only start sync manager after queued blocks are processed
                        _syncManager = new SynchronizationManager(
                            blockTree,
                            blockValidator,
                            headerValidator,
                            transactionStore,
                            txValidator,
                            _networkLogger);

                        _syncManager.Start();

                        await InitNet(initParams.P2PPort ?? 30303).ContinueWith(initNetTask =>
                        {
                            if (initNetTask.IsFaulted)
                            {
                                _networkLogger.Error("Unable to initialize network layer.", initNetTask.Exception);
                            }
                        });

                        // create shared objects between discovery and peer manager
                        _nodeFactory = new NodeFactory();
                        _nodeStatsProvider = new NodeStatsProvider(_configProvider);

                        if (initParams.DiscoveryEnabled)
                        {
                            await InitDiscovery(initParams).ContinueWith(initDiscoveryTask =>
                            {
                                if (initDiscoveryTask.IsFaulted)
                                {
                                    _networkLogger.Error("Unable to initialize discovery protocol.", initDiscoveryTask.Exception);
                                }
                            });
                        }
                        else if (_discoveryLogger.IsInfoEnabled) _discoveryLogger.Info("Discovery protocol disabled");

                        await InitPeerManager().ContinueWith(initPeerManagerTask =>
                        {
                            if (initPeerManagerTask.IsFaulted)
                            {
                                _networkLogger.Error("Unable to initialize peer manager.", initPeerManagerTask.Exception);
                            }
                        });
                    }
                }
            });
        }

        private static ISealEngine ConfigureSealEngine(TransactionStore transactionStore, EthereumSigner ethereumSigner, InitParams initParams)
        {
            var blockMiningTime = TimeSpan.FromMilliseconds(initParams.FakeMiningDelay ?? 12000);
            // var sealEngine = new EthashSealEngine(new Ethash());
            
            var sealEngine = new FakeSealEngine(blockMiningTime, false);
            sealEngine.IsMining = initParams.IsMining ?? false; 
            if (sealEngine.IsMining)
            {
                var transactionDelay = TimeSpan.FromMilliseconds((initParams.FakeMiningDelay ?? 12000) / 4);
                TestTransactionsGenerator testTransactionsGenerator = new TestTransactionsGenerator(transactionStore, ethereumSigner, transactionDelay, _chainLogger);
                // stateProvider.CreateAccount(testTransactionsGenerator.SenderAddress, 1000.Ether());
                // stateProvider.Commit(specProvider.GenesisSpec);
                testTransactionsGenerator.Start();
            }

            return sealEngine;
        }

        private static void LoadGenesisBlock(ChainSpec chainSpec, Keccak expectedGenesisHash, BlockTree blockTree, StateProvider stateProvider, ISpecProvider specProvider)
        {
            // if we already have a database with blocks then we do not need to load genesis from spec
            if (blockTree.Genesis != null)
            {
                return;
            }

            foreach (KeyValuePair<Address, BigInteger> allocation in chainSpec.Allocations)
            {
                stateProvider.CreateAccount(allocation.Key, allocation.Value);
            }

            stateProvider.Commit(specProvider.GenesisSpec);

            Block genesis = chainSpec.Genesis;
            genesis.StateRoot = stateProvider.StateRoot;
            genesis.Hash = BlockHeader.CalculateHash(genesis.Header);

            ManualResetEvent genesisProcessedEvent = new ManualResetEvent(false);
            void GenesisProcessed(object sender, BlockEventArgs args)
            {
                blockTree.NewHeadBlock -= GenesisProcessed;
                genesisProcessedEvent.Set();
            }

            blockTree.NewHeadBlock += GenesisProcessed;
            blockTree.SuggestBlock(genesis);
            genesisProcessedEvent.WaitOne();
            
            // if expectedGenesisHash is null here then it means that we do not care about the exact value in advance (e.g. in test scenarios)
            if (expectedGenesisHash != null && blockTree.Genesis.Hash != expectedGenesisHash)
            {
                throw new Exception($"Unexpected genesis hash, expected {expectedGenesisHash}, but was {blockTree.Genesis.Hash}");
            }
        }

        private async Task InitNet(int listenPort)
        {
            /* tools */
            _messageSerializationService = new MessageSerializationService();
            _cryptoRandom = new CryptoRandom();
            _signer = new Signer();

            /* rlpx */
            var eciesCipher = new EciesCipher(_cryptoRandom);
            var eip8Pad = new Eip8MessagePad(_cryptoRandom);
            _messageSerializationService.Register(new AuthEip8MessageSerializer(eip8Pad));
            _messageSerializationService.Register(new AckEip8MessageSerializer(eip8Pad));
            var encryptionHandshakeServiceA = new EncryptionHandshakeService(_messageSerializationService, eciesCipher, _cryptoRandom, _signer, _privateKey, _networkLogger);

            /* p2p */
            _messageSerializationService.Register(new HelloMessageSerializer());
            _messageSerializationService.Register(new DisconnectMessageSerializer());
            _messageSerializationService.Register(new PingMessageSerializer());
            _messageSerializationService.Register(new PongMessageSerializer());

            /* eth */
            _messageSerializationService.Register(new StatusMessageSerializer());
            _messageSerializationService.Register(new TransactionsMessageSerializer());
            _messageSerializationService.Register(new GetBlockHeadersMessageSerializer());
            _messageSerializationService.Register(new NewBlockHashesMessageSerializer());
            _messageSerializationService.Register(new GetBlockBodiesMessageSerializer());
            _messageSerializationService.Register(new BlockHeadersMessageSerializer());
            _messageSerializationService.Register(new BlockBodiesMessageSerializer());
            _messageSerializationService.Register(new NewBlockMessageSerializer());

            _networkLogger.Info("Initializing server...");
            _localPeer = new RlpxPeer(new NodeId(_privateKey.PublicKey), listenPort, encryptionHandshakeServiceA, _messageSerializationService, _syncManager, _networkLogger);
            await _localPeer.Init();

            var localIp = _networkHelper.GetLocalIp();
            _networkLogger.Info($"Node is up and listening on {localIp}:{listenPort}... press ENTER to exit");
            _networkLogger.Info($"enode://{_privateKey.PublicKey}@{localIp}:{listenPort}");
        }

        private async Task InitPeerManager()
        {
            _networkLogger.Info("Initializing peer manager");

            var peerStorage = new PeerStorage(_configProvider, _nodeFactory, _networkLogger, _perfService);
            var peerManager = new PeerManager(_localPeer, _discoveryManager, _networkLogger, _configProvider, _syncManager, _nodeStatsProvider, peerStorage, _perfService, _nodeFactory);
            await peerManager.Start();

            _networkLogger.Info("Peer manager initialization completed");
        }

        private Task InitDiscovery(InitParams initParams)
        {
            _discoveryLogger.Info("Initializing Discovery");

            if (initParams.DiscoveryPort.HasValue)
            {
                ((NetworkConfig)_configProvider.NetworkConfig).MasterPort = initParams.DiscoveryPort.Value;
            }

            var privateKeyProvider = new PrivateKeyProvider(_privateKey);
            var discoveryMessageFactory = new DiscoveryMessageFactory(_configProvider);
            var nodeIdResolver = new NodeIdResolver(_signer);

            var msgSerializersProvider = new DiscoveryMsgSerializersProvider(_messageSerializationService, _signer, privateKeyProvider, discoveryMessageFactory, nodeIdResolver, _nodeFactory);
            msgSerializersProvider.RegisterDiscoverySerializers();

            var configProvider = new JsonConfigProvider();
            var jsonSerializer = new JsonSerializer(_discoveryLogger);
            var encrypter = new AesEncrypter(configProvider, _discoveryLogger);
            var keyStore = new FileKeyStore(configProvider, jsonSerializer, encrypter, _cryptoRandom, _discoveryLogger);
            var nodeDistanceCalculator = new NodeDistanceCalculator(_configProvider);
            var nodeTable = new NodeTable(_configProvider, _nodeFactory, keyStore, _discoveryLogger, nodeDistanceCalculator);

            var evictionManager = new EvictionManager(nodeTable, _discoveryLogger);
            var nodeLifeCycleFactory = new NodeLifecycleManagerFactory(_nodeFactory, nodeTable, _discoveryLogger, _configProvider, discoveryMessageFactory, evictionManager, _nodeStatsProvider);

            var discoveryStorage = new DiscoveryStorage(_configProvider, _nodeFactory, _discoveryLogger, _perfService);
            _discoveryManager = new DiscoveryManager(_discoveryLogger, _configProvider, nodeLifeCycleFactory, _nodeFactory, nodeTable, discoveryStorage);

            var nodesLocator = new NodesLocator(nodeTable, _discoveryManager, _configProvider, _discoveryLogger);
            _discoveryApp = new DiscoveryApp(_configProvider, nodesLocator, _discoveryLogger, _discoveryManager, _nodeFactory, nodeTable, _messageSerializationService, _cryptoRandom, discoveryStorage);
            _discoveryApp.Start(_privateKey.PublicKey);

            _discoveryLogger.Info("Discovery initialization completed");

            return Task.CompletedTask;
        }
    }
}