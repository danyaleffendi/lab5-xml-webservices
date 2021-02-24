using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace Lab5_XML_WebServices.Controllers
{
    public class BooksController : Controller
    {
        // GET: Books
        public IActionResult Index()
        {
            IList<Models.Book> bookList = new List<Models.Book>();

            //load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                doc.Load(path);
                XmlNodeList books = doc.GetElementsByTagName("book");
                XmlNodeList author = doc.GetElementsByTagName("author");

               /* foreach (XmlElement a in author)
                {
                    Models.Book book = new Models.Book();
                    book.Title = a.GetAttribute("title");
                    bookList.Add(book);
                }*/
                foreach (XmlElement b in books)
                {
                    Models.Book book = new Models.Book();
                    
                    book.ID = Int32.Parse(b.GetElementsByTagName("id")[0].InnerText);
                    book.BookTitle = b.GetElementsByTagName("title")[0].InnerText;
                 
                    book.FirstName = b.GetElementsByTagName("firstname")[0].InnerText;
                    var middle = b.SelectSingleNode("./author/middlename");
                    if (middle != null)
                    {
                        book.MiddleName = b.GetElementsByTagName("middlename")[0].InnerText;
                    }
                    else
                    {
                        book.MiddleName = "";
                    }
                    book.LastName = b.GetElementsByTagName("lastname")[0].InnerText;                   

                    bookList.Add(book);
                }
            }        

            return View(bookList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var book = new Models.Book();
            return View(book);
        }

        [HttpPost]
        public ActionResult Create(Models.Book b)
        {
            //load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                //if file exists, just load it and create new book
                doc.Load(path);

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);

                //get the root element
                doc.DocumentElement.AppendChild(book);

            }
            else
            {
                //file doesn't exist, so create and create new book
                XmlNode dec = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.AppendChild(dec);
                XmlNode root = doc.CreateElement("books");

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);
                root.AppendChild(book);
                doc.AppendChild(root);
            }
            doc.Save(path);

            //return View();
            return RedirectToAction("Index");
        }
        
        private XmlElement _CreateBookElement(XmlDocument doc, Models.Book newBook)
        {
            XmlElement book = doc.CreateElement("book");  
            XmlNode ID = doc.CreateElement("id");
            //Auto incrementing ID
            XmlNodeList id = doc.GetElementsByTagName("id");
            foreach (XmlElement i in id)
            {
                ID.InnerText = (Int32.Parse(i.InnerText) + 1).ToString();
            }

            XmlNode booktitle = doc.CreateElement("title");
            booktitle.InnerText = newBook.BookTitle;

            //Adding Title Attribute in Author Node
            XmlAttribute title = doc.CreateAttribute("title");
            title.InnerText = newBook.Title;

            //Adding Elements in Author Note
            XmlNode author = doc.CreateElement("author");            
            XmlNode first = doc.CreateElement("firstname");
            first.InnerText = newBook.FirstName;
            XmlNode middle = doc.CreateElement("middlename");
            middle.InnerText = newBook.MiddleName;
            XmlNode last = doc.CreateElement("lastname");
            last.InnerText = newBook.LastName;           


            author.Attributes.Append(title);
            author.AppendChild(first);
            author.AppendChild(middle);
            author.AppendChild(last);

            book.AppendChild(ID);
            book.AppendChild(booktitle);
            book.AppendChild(author);

            
            return book;
        }
    }
}