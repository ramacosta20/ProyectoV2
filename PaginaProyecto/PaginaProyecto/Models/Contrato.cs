using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Xunit;

namespace PaginaProyecto.Models
{
    public class Contrato
    {
        [Fact]
        public async Task ShouldBeAbleToDeployAContract()
        {
            var password = "password";
            var abi = @"[{""constant"":false,""inputs"":[{""name"":""direcDestinatario"",""type"":""address""}],""name"":""donate"",""outputs"":[{""name"":""retorno"",""type"":""bool""}],""payable"":true,""type"":""function""}]";
            var byteCode =
              "0x6060604052341561000c57fe5b5b6101508061001c6000396000f300606060405263ffffffff60e060020a600035041662362a95811461002b5780633ccfd60b14610053575bfe5b61003f600160a060020a0360043516610065565b604080519115158252519081900360200190f35b341561005b57fe5b6100636100d0565b005b600160a060020a038116600090815260016020818152604080842080543401908190558483529084205492909152106100c65750600160a060020a0381166000908152600160208181526040808420546002909252909220919091556100ca565b5060005b5b919050565b600160a060020a033316600081815260026020818152604080842080546001845282862086905593909252908390555190929183156108fc02918491818181858888f19350505050151561012057fe5b5b505600a165627a7a723058200ca2894f66f4b6d9d95c0b0e470eebbba48b39b963c0f1911eed74050d80ab160029";

            var emisor = "0xb6747110b8d1d0038fe250f9b9c2a0cb348fff57";
            var receptor = "0x74805a06192899214083bb0abe7efffc403a0c61";

            var web3 = new Nethereum.Web3.Web3();
            //var unlockAccountResult =
                //await web3.Personal.UnlockAccount.SendRequestAsync(receptor, password, 120);
            //Assert.True(unlockAccountResult);

            var transactionHash =
                await web3.Eth.DeployContract.SendRequestAsync(byteCode, emisor, new HexBigInteger(500000), new HexBigInteger(1));

            var mineResult = await web3.Miner.Start.SendRequestAsync(6);

            Assert.True(mineResult);

            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (receipt == null)
            {
                Thread.Sleep(5000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }

            mineResult = await web3.Miner.Stop.SendRequestAsync();
            Assert.True(mineResult);

            var contractAddress = receipt.ContractAddress;

            var contract = web3.Eth.GetContract(abi, contractAddress);

            var funcionDonar = contract.GetFunction("donate");

            var hashTransaccion = await funcionDonar.SendTransactionAsync(emisor,new HexBigInteger(20), new HexBigInteger(1), new HexBigInteger(30));
            

            //var receibo = await MineAndGetReceiptAsync(web3, hashDonacion);
        }
    }
}