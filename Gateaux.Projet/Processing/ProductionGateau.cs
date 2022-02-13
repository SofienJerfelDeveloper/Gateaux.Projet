using Gateaux.Projet.Models;
using Gateaux.Projet.Models.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Gateaux.Projet.Processing
{
    internal class ProductionGateau
    {
        private ConcurrentBag<Gateau> _gateauList;
        private DataflowLinkOptions _dataflowLinkOptions;
        private ExecutionDataflowBlockOptions _datapreparationOptions;
        private ExecutionDataflowBlockOptions _datacuissonOptions;
        private ExecutionDataflowBlockOptions _dataemballageOptions;

        private Timer _timer;

        public ProductionGateau()
        {
            _gateauList = new ConcurrentBag<Gateau>();
            _dataflowLinkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            _datapreparationOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 }; //Je peux préparer 3 gâteaux en même temps
            _datacuissonOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 };     //Je peux cuire 5 gâteaux en même temps
            _dataemballageOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 };   //Je peux emballer 2 gâteaux en même temps
        }

        public async Task RunAsync(int nombreMaximumGateau,int dureeAffichageEcran=60)
        {
            _timer = new Timer(AfficherAvancement, null, TimeSpan.Zero,TimeSpan.FromSeconds(dureeAffichageEcran));

            var preparation = new TransformBlock<Gateau, Gateau>(async recette => await PreparerGateauAsync(), _datapreparationOptions);
            var cuisson = new TransformBlock<Gateau, Gateau>(async gateau => await CuisinerGateauAsync(gateau), _datacuissonOptions);
            var emballage = new TransformBlock<Gateau, Gateau>(async gateau => await EmballageGateauAsync(gateau), _dataemballageOptions);
            var Fini = new ActionBlock<Gateau>(async gateau => await FiniGateauAsync(gateau));

            preparation.LinkTo(cuisson, _dataflowLinkOptions);
            cuisson.LinkTo(emballage, _dataflowLinkOptions);
            emballage.LinkTo(Fini, _dataflowLinkOptions);

            int i = 0;
            
            while (i< nombreMaximumGateau)
            {
                i++;
                await preparation.SendAsync(new Gateau());
            }

            preparation.Complete();
            await preparation.Completion;
            await cuisson.Completion;
            await emballage.Completion;
            _timer.Dispose();
        }

        private void AfficherAvancement(object state)
        {

            Console.WriteLine($"Nombre des gâteaux en cours de préparation : {_gateauList.Where(x => x.Status == StatusGateauEnum.preparation).Count()}");
            Console.WriteLine($"Nombre des gâteaux en cours de cuisson     : {_gateauList.Where(x => x.Status == StatusGateauEnum.cuisson).Count()}");
            Console.WriteLine($"Nombre des gâteaux en cours d'emballage    : {_gateauList.Where(x => x.Status == StatusGateauEnum.emballage).Count()}");
            Console.WriteLine($"Total des gâteaux finis : {_gateauList.Where(x => x.Status == StatusGateauEnum.fini).Count()}");

            Console.WriteLine("");
        }

    
        private async Task<Gateau> PreparerGateauAsync()
        {
            Random random = new Random();
            int dureePrep = random.Next(5, 9);

            await Task.Delay(dureePrep*1000);
            var gateau = new Gateau();
            _gateauList.Add(gateau);
            return gateau;
        }

        private async Task<Gateau> CuisinerGateauAsync(Gateau gateau)
        {
            int dureeCuisson = 10;

            await Task.Delay(dureeCuisson*1000 );
            gateau.Status = StatusGateauEnum.cuisson;
            return gateau;
        }

        private async Task<Gateau> EmballageGateauAsync(Gateau gateau)
        {
            int dureeEmbalage = 2;

            await Task.Delay(dureeEmbalage *1000);
            gateau.Status = StatusGateauEnum.emballage;
            return gateau;
        }


        private async Task<Gateau> FiniGateauAsync(Gateau gateau)
        {

            gateau.Status = StatusGateauEnum.fini;
            return gateau;
        }


    }
}
