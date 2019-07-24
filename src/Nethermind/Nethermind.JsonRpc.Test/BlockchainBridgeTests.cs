using Nethermind.Blockchain;
using Nethermind.Blockchain.Filters;
using Nethermind.Blockchain.Receipts;
using Nethermind.Blockchain.TxPools;
using Nethermind.Blockchain.TxPools.Storages;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Specs;
using Nethermind.Core.Test.Builders;
using Nethermind.Evm;
using Nethermind.Facade;
using Nethermind.Logging;
using Nethermind.Store;
using Nethermind.Wallet;
using NSubstitute;
using NUnit.Framework;

namespace Nethermind.JsonRpc.Test
{
    [TestFixture]
    public class BlockchainBridgeTests
    {
        [Test]
        public void Test()
        {
            int chainId = 0xcac;
            
            Block genesis = Build.A.Block.Genesis.TestObject;
            IBlockTree blockTree = Substitute.For<IBlockTree>();
            blockTree.ChainId.Returns(chainId);
            
            blockTree.Head.Returns(genesis.Header);
            MainNetSpecProvider specProvider = MainNetSpecProvider.Instance;
            specProvider.ChainId = chainId;
            
            TxPool txPool = new TxPool(
                NullTxStorage.Instance,
                new Timestamp(),
                new EthereumEcdsa(specProvider, LimboLogs.Instance),
                specProvider, new TxPoolConfig(), LimboLogs.Instance);
            
            BlockchainBridge blockchainBridge = new BlockchainBridge(
                Substitute.For<IStateReader>(),
                Substitute.For<IStateProvider>(),
                Substitute.For<IStorageProvider>(),
                blockTree,
                txPool, new TxPoolInfoProvider(Substitute.For<IStateProvider>()), new InMemoryReceiptStorage(),
                Substitute.For<IFilterStore>(),
                Substitute.For<IFilterManager>(),
                new DevWallet(specProvider, new WalletConfig(), LimboLogs.Instance),
                Substitute.For<ITransactionProcessor>(),
                new EthereumEcdsa(specProvider, LimboLogs.Instance));

            var sender = new Address("7e5f4552091a69125d5dfcb7b8c2659029395bdf");
            Transaction transaction = new Transaction();
            transaction.SenderAddress = sender;

            blockchainBridge.Sign(transaction);
            blockchainBridge.SendTransaction(transaction);

            var txs = txPool.GetPendingTransactions();
            Assert.AreEqual(sender, transaction.SenderAddress);
        }
    }
}