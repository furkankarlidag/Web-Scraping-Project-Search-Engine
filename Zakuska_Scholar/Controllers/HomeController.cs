using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Threading;
using System.Xml;
using Zakuska_Scholar.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Elasticsearch.Net;
using static System.Net.Mime.MediaTypeNames;

namespace Zakuska_Scholar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public static searchDto searchDtos = new searchDto();
        static int devam = 0;
        static int counter = 0;
        searchDto dto2 = new searchDto();
        public static List<articleModel> modeller = new List<articleModel>();
        public static List<articleModel> modeller2 = new List<articleModel>();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            dbFunctions dbFunctions = new dbFunctions();
            var realModel = new articleModel
            {
                articleID = 8888,
                articleName = "bos",
                nameOfWriters = "bos",
                typeOfArticle = "bos",
                timeOfPublish = "bos",
                nameOfPublisher = "bos",
                searchKeys = "bos",
                articleKeys = "bos",
                summary = "bos",
                references = "bos",
                numOfQuote = 8888,
                url = "bos"
            };
            dbFunctions.birTaneEkle(realModel);



            dto2.articles = dbFunctions.MongodbArticle();
            return View("Index", dto2);

        }

        [HttpGet]
        public IActionResult Detay(int id)
        {
            if (counter == 0)
            {
                dbFunctions dbFunctions = new dbFunctions();
                dto2.articles = dbFunctions.MongodbArticle();
                if (dto2.articles != null)
                {
                    foreach (var item in dto2.articles)
                    {
                        if (item.articleID == id)
                            return View(item);
                    }
                }
                return View();
            }
            else
            {
                if (searchDtos.articles != null)
                {
                    foreach (var item in searchDtos.articles)
                    {
                        if (item.articleID == id)
                            return View(item);
                    }
                }
                return View();
            }

        }
        [HttpGet]
        public IActionResult wrongResult(string str)
        {
            searchDtos = new searchDto();
            searchDtos.search = str;
            devam = 1;
            searchDtos.duzeltilmisSearch = "";
            return RedirectToAction("GetArticle", "Home", searchDtos);
        }

        [HttpPost]
        public IActionResult SearchPost(string submitButton, string search, string selectedYear, string selectedType, string selectedSort)
        {

            if (submitButton == "search")
            {
                counter++;
                searchDtos = new searchDto();
                searchDtos.search = search;
                devam = 1;

                string control = ""; // Varsayılan değer atanıyor

                string url = $"https://scholar.google.com/scholar?hl=tr&as_sdt=0%2C5&q={WebUtility.UrlEncode(search)}";

                WebClient webClient = new WebClient();
                string html = webClient.DownloadString(url);

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                HtmlNodeCollection linkNodes = document.DocumentNode.SelectNodes("//h2[@class='gs_rt']/a");

                if (linkNodes != null)
                {
                    foreach (HtmlNode linkNode in linkNodes)
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(linkNode.InnerHtml);
                        control = doc.DocumentNode.InnerText;
                        break; // Döngüden çıkılıyor
                    }
                }

                searchDtos.duzeltilmisSearch = control;
                return RedirectToAction("GetArticle", "Home", searchDtos);
            }
            else if (submitButton == "applyFilters1")
            {
                List<articleModel> models = new List<articleModel>();
                devam = 2;

                if (searchDtos.articles != null && selectedYear != null)
                {
                    foreach (var item in searchDtos.articles)
                    {
                        if (item.timeOfPublish == selectedYear)
                            models.Add(item);
                    }
                }

                if (models.Count > 0)
                {
                    searchDtos.articles = models;
                    return RedirectToAction("GetArticle", "Home");
                }

            }
            else if (submitButton == "removeFilters")
            {

                searchDtos.articles = modeller;
                return RedirectToAction("GetArticle", "Home", searchDtos);
            }
            else if (submitButton == "applyFilters2")
            {
                List<articleModel> models = new List<articleModel>();
                devam = 2;

                if (searchDtos.articles != null && selectedType != null)
                {
                    foreach (var item in searchDtos.articles)
                    {
                        if (item.typeOfArticle == selectedType)
                            models.Add(item);
                    }
                }

                if (models.Count > 0)
                {
                    searchDtos.articles = models;
                    return RedirectToAction("GetArticle", "Home");
                }

            }
            else if (submitButton == "sort")
            {
                devam = 2;
                if (selectedSort == "Yıl")
                {
                    if (searchDtos.articles != null)
                    {
                        int n = searchDtos.articles.Count;
                        for (int i = 0; i < n - 1; i++)
                        {
                            for (int j = 0; j < n - i - 1; j++)
                            {
                                int yil1, yil2;
                                int.TryParse(searchDtos.articles[j].timeOfPublish, out yil1);
                                int.TryParse(searchDtos.articles[j + 1].timeOfPublish, out yil2);
                                if (yil1 < yil2)
                                {
                                    articleModel temp = searchDtos.articles[j];
                                    searchDtos.articles[j] = searchDtos.articles[j + 1];
                                    searchDtos.articles[j + 1] = temp;
                                }
                            }
                        }
                    }
                    return RedirectToAction("GetArticle", "Home");
                }
                else if (selectedSort == "Alıntı Sayısı")
                {
                    if (searchDtos.articles != null)
                    {
                        int n = searchDtos.articles.Count;
                        for (int i = 0; i < n - 1; i++)
                        {
                            for (int j = 0; j < n - i - 1; j++)
                            {
                                if (searchDtos.articles[j].numOfQuote < searchDtos.articles[j + 1].numOfQuote)
                                {
                                    articleModel temp = searchDtos.articles[j];
                                    searchDtos.articles[j] = searchDtos.articles[j + 1];
                                    searchDtos.articles[j + 1] = temp;
                                }
                            }
                        }
                    }
                    return RedirectToAction("GetArticle", "Home");
                }
            }

            return View("Index");
        }

        [HttpGet]
        public IActionResult GetArticle(searchDto Dtos)
        {
            if (devam == 1)
            {
                searchDto dto = new searchDto();
                Thread t = new Thread(() => dto = WebScraping(Dtos.search));
                t.Start();
                t.Join();

                modeller = dto.articles;

                return View("Index", dto);
            }
            else if (counter == 0)
            {
                dbFunctions dbFunctions = new dbFunctions();
                dto2.articles = dbFunctions.MongodbArticle();
                return View("Index", dto2);
            }
            else if (devam == 2)
            {
                return View("Index", searchDtos);
            }

            else
                return View("Index");

        }


        static searchDto WebScraping(string search)
        {
            dbFunctions dbFunctions = new dbFunctions();

            dbFunctions.deleteAll();
            //dbFunctions.deleteFromElastic();
            searchDtos.articles = new List<articleModel>();
            if (search.Contains(" "))
                search = search.Replace(" ", "+");
            // Dergipark.org.tr'deki arama sonuçlarını çekeceğimiz URL
            string url = $"https://dergipark.org.tr/tr/search?q={search}&section=articles";

            // Html dokümanını indir
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            Console.WriteLine(doc.ToString());

            // Makaleleri seçmek için XPath kullanıyoruz
            HtmlNodeCollection articleNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'card article-card')]");
            string yayinAdi = null;

            if (articleNodes != null)
            {
                // Makale sayısını ve isimlerini ekrana yazdır
                Console.WriteLine("Toplam makale sayısı: " + articleNodes.Count);
                Console.WriteLine();

                foreach (HtmlNode articleNode in articleNodes)
                {
                    articleModel article = new articleModel();
                    article.searchKeys = search;
                    string pdfUrl = null;
                    // Makale başlığını seç
                    HtmlNode titleNode = articleNode.SelectSingleNode(".//h5/a");
                    HtmlNode typeNode = articleNode.SelectSingleNode(".//span[contains(@class, 'badge badge-secondary')]");
                    HtmlNodeCollection authorNodes = articleNode.SelectNodes(".//small[contains(@class, 'article-meta')]/a[contains(@class, 'fw-500')]");

                    string url2 = titleNode.Attributes["href"].Value; ;
                    HtmlWeb web2 = new HtmlWeb();
                    if (web2.Load(url2) == null)
                        continue;
                    HtmlDocument doc2 = web.Load(url2);
                    article.url = url2;
                    HtmlNodeCollection publishNodes = doc2.DocumentNode.SelectNodes("//div[contains(@class, 'kt-heading kt-align-center')]");
                    HtmlNodeCollection paneNodes = doc2.DocumentNode.SelectNodes("//span[contains(@class, 'article-subtitle')]");
                    string journal;
                    if (publishNodes == null)
                        continue;
                    foreach (HtmlNode publishNode in publishNodes)
                    {
                        HtmlNode journalNode = publishNode.SelectSingleNode(".//h1[@id='journal-title']");
                        journal = journalNode.InnerText.Trim();
                        yayinAdi = journal;
                        Console.WriteLine("Journal: " + journal);
                        article.nameOfPublisher = yayinAdi;
                    }
                    string year;
                    foreach (HtmlNode yearNode in paneNodes)
                    {
                        HtmlNode yearsNode = yearNode.SelectSingleNode(".//a");
                        year = yearsNode.InnerText.Trim();
                        Console.WriteLine("Yıl: " + year);
                        string years = year.Substring(year.IndexOf(" ") + 1);
                        article.timeOfPublish = years;
                    }
                    string abs = "";
                    HtmlNodeCollection abstractsNodes = doc2.DocumentNode.SelectNodes("//div[contains(@class, 'article-abstract data-section')]");
                    if (!(abstractsNodes != null && abstractsNodes.Count > 1))
                        continue;
                    HtmlNode abstractNode = abstractsNodes[1];

                    HtmlNode absNode = abstractNode.SelectSingleNode(".//p");
                    abs = absNode.InnerText.Trim();
                    if (abs == "")
                    {
                        abstractsNodes = doc2.DocumentNode.SelectNodes("//div[contains(@class, 'article-abstract data-section')]");
                        abstractNode = abstractsNodes[1];
                        absNode = abstractNode.SelectSingleNode(".//p[contains(@class, 'MsoNormal')]");
                        if (absNode == null)
                        {
                            HtmlNode abst = abstractsNodes[0];
                            HtmlNode abstr = abst.SelectSingleNode(".//p");
                            abs = abstr.InnerText.Trim();

                        }
                        if (abs == "")
                        {
                            foreach (HtmlNode abst in abstractsNodes)
                            {
                                HtmlNodeCollection abstr = abst.SelectNodes(".//p");
                                foreach (var item in abstr)
                                {
                                    if (item.InnerText.Trim() != null)
                                    {
                                        abs = item.InnerText.Trim();
                                        break;
                                    }

                                }
                            }
                            if (abs == "")
                            {
                                foreach (var item in abstractsNodes)
                                {
                                    HtmlNode sp = item.SelectSingleNode(".//span");
                                    if (sp != null)
                                    {
                                        abs = sp.InnerText.Trim();
                                        break;
                                    }

                                }
                            }
                        }

                    }
                    Console.WriteLine("Özet: " + abs);
                    article.summary = abs;


                    HtmlNodeCollection keysNodes = doc2.DocumentNode.SelectNodes("//div[contains(@class, 'article-keywords data-section')]");
                    if (keysNodes == null)
                    {
                        continue;
                    }
                    ArrayList keywords = new ArrayList();
                    string kyw = null;
                    foreach (HtmlNode keyNode in keysNodes)
                    {
                        HtmlNodeCollection keywordNode = keyNode.SelectNodes(".//a");
                        foreach (HtmlNode kywordNode in keywordNode)
                        {
                            keywords.Add(kywordNode.InnerText.Trim());
                        }
                    }
                    Console.WriteLine("Anahtar Kelimeler:");
                    foreach (string keywrd in keywords)
                    {

                        Console.WriteLine("-" + keywrd);
                        if (kyw != null)
                        {
                            kyw = kyw + "," + keywrd;
                        }
                        else
                            kyw = keywrd;
                    }
                    article.articleKeys = kyw;



                    HtmlNodeCollection ReferencesNodes = doc2.DocumentNode.SelectNodes("//ul[contains(@class, 'fa-ul')]//li");
                    ArrayList references = new ArrayList();
                    if (ReferencesNodes != null)
                    {
                        foreach (HtmlNode ilNode in ReferencesNodes)
                        {
                            if (references.Contains(ilNode.InnerText.Trim()))
                                break;
                            references.Add(ilNode.InnerText.Trim());
                        }
                        string rfr = null;
                        Console.WriteLine("Kaynakça:");
                        foreach (string refe in references)
                        {
                            Console.WriteLine("-" + refe);
                            if (rfr != null)
                            {
                                rfr = rfr + "," + refe;
                            }
                            else
                                rfr = refe;
                        }
                        article.references = rfr;
                    }
                    else
                        article.references = "Yok";


                    HtmlNodeCollection citedByNodes = doc2.DocumentNode.SelectNodes("//div[contains(@class, 'mt-3')]//a");
                    int alintiSayisi = 0;
                    if (citedByNodes != null)
                    {
                        foreach (HtmlNode aNode in citedByNodes)
                        {
                            Console.WriteLine("Alıntı Sayısı:" + aNode.InnerText.Trim());
                            string numberString = aNode.InnerText.Trim().Split(':')[1].Trim();
                            bool success = int.TryParse(numberString, out alintiSayisi);
                        }
                    }
                    article.numOfQuote = alintiSayisi;

                    ArrayList authorsList = new ArrayList();
                    if (titleNode != null)
                    {
                        string title = titleNode.InnerText.Trim();
                        article.articleName = title;
                        if (typeNode == null)
                            continue;

                        string type = typeNode.InnerText.Trim();
                        article.typeOfArticle = type;

                        Console.WriteLine("Makale Başlığı: " + title);
                        Console.WriteLine("Makale Türü: " + type);
                        if (authorNodes != null)
                        {
                            Console.WriteLine("Yazarlar:");
                            string yazarlar = null;
                            foreach (HtmlNode authorNode in authorNodes)
                            {
                                if (yayinAdi == authorNode.InnerText.Trim())
                                    break;
                                string author = authorNode.InnerText.Trim();
                                authorsList.Add(author);
                                Console.WriteLine("- " + author);
                                if (yazarlar != null)
                                {
                                    yazarlar = yazarlar + "," + author;
                                }
                                else
                                    yazarlar = author;
                            }
                            article.nameOfWriters = yazarlar;
                        }
                        else
                        {
                            Console.WriteLine("Yazarlar bulunamadı.");
                        }

                        HtmlNodeCollection divNodes = doc2.DocumentNode.SelectNodes("//div[contains(@id, 'article-toolbar')]");
                        if (divNodes != null && divNodes.Any())
                        {
                            foreach (HtmlNode divNode in divNodes)
                            {
                                HtmlNode aNode = divNode.SelectSingleNode(".//a[contains(@class, 'btn btn-sm float-left article-tool pdf d-flex align-items-center')]");

                                if (aNode != null && aNode.Attributes["href"] != null)
                                {
                                    string hrefValue = aNode.Attributes["href"].Value;
                                    pdfUrl = "https://dergipark.org.tr" + hrefValue;
                                    Console.WriteLine("Href: " + hrefValue);
                                }
                            }
                            string path = $"C:\\Users\\Victus\\OneDrive\\Masaüstü\\PDF\\{article.articleName}.pdf";
                            DownloadPDF(pdfUrl, path);
                            article.summaryabs = article.summary.Substring(0, Math.Min(200, article.summary.Length));
                            //searchDtos.articles.Add(article);
                            dbFunctions.insertData(article);
                            dbFunctions.insertDataForMain(article);
                            Thread.Sleep(1000);
                        }
                    }
                    Console.WriteLine();
                }
                dbFunctions.ElasticConnect();
                List<articleModel> deneme = dbFunctions.GetAllDocumentsFromElastic();
                searchDtos.articles = deneme;

            }
            else
            {
                Console.WriteLine("Makaleler bulunamadı.");
            }
            return searchDtos;
        }

        static void DownloadPDF(string url, string fileName)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] pdfBytes = client.DownloadData(url);
                    System.IO.File.WriteAllBytes(fileName, pdfBytes);
                    Console.WriteLine($"{fileName} başarıyla indirildi.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Hata: " + e.Message);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}