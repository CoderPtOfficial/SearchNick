using HtmlAgilityPack;
using Microsoft.SqlServer.Server;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NickSearch
{
    class Program
    {
      
        static void Main(string[] args)
        {
            Menu();
            
        }
        private static void Menu()
        {
            Console.Clear();
            Console.WriteLine("O que desja fazer?");
            Console.WriteLine("(1) Procurar nick\n" +
                "(2) Ler ficheiro\n"+"(3) Sair");

            string nosense=Console.ReadLine();
            Console.WriteLine(nosense.Length);
            int op = 0;
            if (nosense.Length > 0) op = int.Parse(nosense);
            else Menu();

            switch (op)
            {
                case 1:
                    Search();
                    break;
                case 2:
                    ReadFile();
                    break;
                case 3:
                    return;
                default:
                    Menu();
                    break;
            }
        }

        private static void Search()
        {
            Console.WriteLine("Qual nick deseja procurar?");
            string searchNick = Console.ReadLine();

            if (searchNick.Length < 1) Search();

            List<String> fbId = new List<string>();
            fbId=Facebook(searchNick);

            string instaId = Insta(searchNick);

            string twitterId=Twitter(searchNick);

            AfterSearch(fbId, instaId, twitterId);
        }

        private static void AfterSearch(List<String> fbId,string instaId,string twitterId)
        {
            Console.Clear();
            Console.WriteLine("Nick obtido, escolha uma opcao\n" +
                "(1) Mostre os resultados\n" +
                "(2) Salve para\n" +
                "(3) Ignorar (perderá tudo)");
            int op = int.Parse(Console.ReadLine());
            switch (op)
            {
                case 1:
                    ShowNicks(fbId,instaId,twitterId);
                    break;
                case 2:
                    SaveNicks(fbId,instaId,twitterId);
                    break;
                case 3:
                    Menu();
                    break;
            }
        }

        private static void ShowNicks(List<String> fbId, string instaId, string twitterId)
        {
            Console.Clear();
            Console.WriteLine("Facebook");
            foreach(string id in fbId)
            {
                Console.WriteLine(id);
            }
            Console.WriteLine("Instagram");
            Console.WriteLine(instaId);
            Console.WriteLine("Twitter");
            Console.WriteLine(twitterId);
            Console.WriteLine("\n");
            Console.WriteLine("Clique em Enter");
            Console.ReadLine();
            AfterSearch(fbId, instaId, twitterId);
        }
        private static void SaveNicks(List<String> fbId, string instaId, string twitterId)
        {
            Console.Clear();
            Console.WriteLine("Qual é o nome do arquivo?");
            
            string fileName = Console.ReadLine();
            if (fileName.Length < 1) SaveNicks(fbId, instaId, twitterId);
            using (StreamWriter file = new StreamWriter(fileName + ".txt"))
            {
                foreach(string id in fbId)
                {
                    file.WriteLine(id);
                }
                file.WriteLine(instaId);
                file.WriteLine(twitterId);
                file.Close();
            }
            
            Console.WriteLine("Salvo em " +
                AppDomain.CurrentDomain.BaseDirectory.ToString()+
                "\n Precione alguma tecla");
            Console.ReadLine();
            Menu();
        }

        private static void ReadFile()
        {
            Console.Clear();
            Console.WriteLine("Qual arquivo deseja ler");
            string fileName = Console.ReadLine();
            if (!File.Exists(fileName + ".txt")) ReadFile();
            using(StreamReader file=new StreamReader(fileName + ".txt"))
            {
                string line = file.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    line = file.ReadLine();
                }
            }
            Console.WriteLine("Clique em alguma tecla");
            Console.ReadLine();
            Menu();
        }

        private static List<String> Facebook(string Nick)
        {
            Console.WriteLine("Comecando com Facebook...");
            //Inicia lista
            List<String> Result = new List<string>(); 
            //Cria Url
            string Url = "https://www.facebook.com/public/"+Nick;
            //Inicia constructor
            HtmlWeb web = new HtmlWeb();
            //Carrega pagina
            HtmlDocument html = web.Load(Url);
            //Obtem HTML
            string htmlFile=html.DocumentNode.InnerHtml;
            //Obtem index da entity_id
            int index = htmlFile.IndexOf("entity_id=");
            //Obtem index depois do =
            int NewIndex = index + 10;
            //Enquantp existir entity_id
            while (index != -1)
            {
                //Cria uma string Builder
                StringBuilder newId = new StringBuilder();
                //Enquanto o char for diferente de &
                while (htmlFile[NewIndex] != '&')
                {
                    //Acrescenta ao builder
                    newId.Append(htmlFile[NewIndex]);
                    //Incrementa o index de procura
                    NewIndex++;
                }
                //Obtem a string do builder
                string id = newId.ToString();
                //Console.WriteLine(id);
                //Apaga o entity_id do id obtido
                htmlFile=htmlFile.Replace("entity_id="+id, "");
                //Obtem denovo o index de entity_id
                index = htmlFile.IndexOf("entity_id=");
                //Obtem o index depois do =
                NewIndex = index + 10;
                //Adiciona à lista
                id = "www.facebook.com/" + id;

                Result.Add(id);
            }

            Console.WriteLine("Facebook terminado");
            return Result;
        }
        private static string Insta(string Nick)
        {
            Console.WriteLine("Instagram....");
            string Result=String.Empty;

            string Url = "https://www.instagram.com/";
            Url += Nick;

            //Inicia constructor
            HtmlWeb web = new HtmlWeb();
            //Carrega pagina
            HtmlDocument html = web.Load(Url);
            //Obtem HTML
            string htmlFile = html.DocumentNode.InnerHtml;
            
            if (htmlFile.Contains("og:url")) Result=Url;
            else Result="N/A";
            Console.WriteLine("Fim Instagram ");
            return Result;
        }

        private static string Twitter(string Nick){
            Console.WriteLine("Twitter....");
            string Result = String.Empty;

            string Url = "https://twitter.com/"+Nick;

            //Inicia constructor
            HtmlWeb web = new HtmlWeb();
            //Carrega pagina
            HtmlDocument html = web.Load(Url);
            //Obtem HTML
            string htmlFile = html.DocumentNode.InnerHtml;
            if (htmlFile.Contains("Sorry, that page doesn't exist")) Result = "N/A";
            else Result = Url;
            Console.WriteLine("Fim Twitter");
            return Result;
        }
    }
}
