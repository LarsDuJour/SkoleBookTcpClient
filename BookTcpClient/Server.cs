using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BookClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BookTcpClient
{
    class Server
    {
        private static List<Book> _books = new List<Book>()
        {
            new Book("Titlen 1","forfatter",20,"1234567890123"),
            new Book("Titlen 2","forfatter",20,"2234567890123"),
            new Book("Titlen 3","forfatter",20,"3234567890123"),
            new Book("Titlen 4","forfatter",20,"4234567890123")
        };

        public Server()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 4646);

            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                Task.Run(() =>
                {
                    doClient(client);
                });
            }
        }

        private void doClient(TcpClient client)
        {
            using (NetworkStream ns = client.GetStream())
            using (StreamWriter sw = new StreamWriter(ns))
            using (StreamReader sr = new StreamReader(ns))
            {
                string input = sr.ReadLine();

                switch (input)
                {
                    case "Hent":
                        string isbnInput = sr.ReadLine();
                        string output = Hent(isbnInput);
                        sw.WriteLine(output);
                        sw.Flush();
                        break;
                    case "HentAlle":
                        string tomInput = sr.ReadLine();
                        output = HentAlle();
                        sw.WriteLine(output);
                        sw.Flush();
                        break;
                    case "Gem":
                        string JsonInput = sr.ReadLine();
                        Gem(JsonInput);
                        break;
                }

            }
        }

        private string Hent(string isbn)
        {
            Book book = _books.Find(b => b.Isbn == isbn);
            string JsonString = "";
            if (book != null)
            {
                JsonString = JsonConvert.SerializeObject(book);
            }
            return JsonString;
        }

        private string HentAlle()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Book book in _books)
            {
                string JsonString = JsonConvert.SerializeObject(book);
                sb.Append(JsonString);
            }

            return sb.ToString();
        }

        private void Gem(string JsonBook)
        {
            Book b = JsonConvert.DeserializeObject<Book>(JsonBook);
            _books.Add(b);
        }
    }
}
