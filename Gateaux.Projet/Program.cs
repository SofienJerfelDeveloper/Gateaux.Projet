using Gateaux.Projet.Processing;
using System;
using System.Threading.Tasks;

namespace Gateaux.Projet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProductionGateau productionGateau = new ProductionGateau();
            int nombreMaximumGateau = SaisirNombreGateaux();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("Relevé affiché chaque minutes  ");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");
            await productionGateau.RunAsync(nombreMaximumGateau);
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("Terminé !");
        }


        static int SaisirNombreGateaux()
        {
            Console.Write("Saisir le nombre maximum des gateaux autorisé dans l'usine : ");
            try
            {
                int nombreMaximumGateau = Int32.Parse(Console.ReadLine());
                return nombreMaximumGateau;
            }
            catch (Exception)
            {
                Console.WriteLine("Erreur : Entrer un nombre valide !!!\n");
            }

            return SaisirNombreGateaux();
        }

    }
}