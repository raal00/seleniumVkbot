/********************************************************************************
*Project name :Lab1                                                             *
*Project type :С# Console Application                                           *
*File name : Program.cs                                                         *
*Language : C#                                                                  *
*Programmer: Raal: C# C++ Java developer                                        *
*Created :19.11.2018                                                            *  
*Last revision :19.11.2018                                                      *
*Comment : selenium vk.com bot                                                  *
*********************************************************************************/


using System;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Interactions;
namespace test_selenium
{
    class Program
    {
        private static readonly int MaxLinks = 6;                 //Maximum links for spam
        private static string login = null, pass = null, text = null;  //your login&password for bot auth as fields
        private static int timer;    //delay 
        enum MOD               
        {
            persMessage = 0,    //pers. message
            comMessage = 1,     //spam to groups
        }

        static void Main(string[] args)
        {
            int choose;       //mod value
            int i,j,k;        //just values for cycles
            text = null;      //your login&password for bot auth as values
            login = null;
            pass = null;

            for (i = 0; i < 10; i++) //snowfall :D
            {
                for (j = 0; j <= i; j++)
                {
                    for (k = 0; k < i - j; k++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write("*");
                }
                Console.Write("\n");
            }
            do                                  // enter Login & password & delay 
            {
                if (login == null)
                {
                    Console.Write("\tEnter login: ");
                    login = Console.ReadLine();
                }
                if (pass == null)
                {
                    Console.Write("\n\tEnter password: ");
                    pass = Console.ReadLine();
                }
                if (text == null)
                {
                    Console.Write("\n\t:Enter message: ");
                    text = Console.ReadLine();
                }
                Console.Write("\nEnter timeout (delay) (sec): ");
                timer = Convert.ToInt32(Console.ReadLine());
            }
            while (login==null || pass==null || text==null);
            Console.WriteLine("0-spam to groups\n1-spam to private message\n");
            do                                                              //enter program mod 
            {
                choose = Convert.ToInt32(Console.ReadLine());
            }
            while (choose < 0 && choose > 1);       //we've got int value and then 
            if (choose == 0)                         //we launch method
                Start(MOD.comMessage);
            else if (choose == 1)
                Start(MOD.persMessage);
            Console.ReadKey();
        }

        static void Start(MOD operation)
        {
            switch (operation)
            {
                case MOD.persMessage:           //pers. message mod creates standart constructor and launches spam
                    {
                        seleniumHundler hundler = new seleniumHundler(login, pass, text, timer);
                        hundler.navTo();
                        break;
                    }
                case MOD.comMessage:     //spam to groups 
                    {
                        int count;            //enter int values of cycles 
                        do                     //every time we'll change link and repeat spam 
                        {
                            Console.WriteLine("how many groups you will spam?\tcount: {1..6}\ncount = ");
                            count = Convert.ToInt32(Console.ReadLine());
                        }
                        while (count > MaxLinks || count <= 0);
                        Xmlelement[] elmnt = new Xmlelement[count];          //struct contains 2 params (link & int count of comments)
                        xmlParser xmlParser = new xmlParser("XMLFile_1.xml");  //create struct for reading from xml file
                        Thread.Sleep(1000);
                        for (int i = 0; i < count; i++)                    //initialize vector from xml file 
                        {
                            elmnt[i] = xmlParser.GetLink(i.ToString());
                            Console.WriteLine($"Link: {elmnt[i].link} Count: {elmnt[i].count}");
                        }

                        Console.WriteLine("Just wait a while...");
                            Thread.Sleep(1000);
                            seleniumHundler hundler = new seleniumHundler(login, pass, text, timer, elmnt); //create overloaded constructor
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Exception in started data");
                        break;
                    }
            }
        }
    }
    
    abstract class BASEselenium //just base constructor for our future constructors 
    {
        protected static readonly int maxCount = 4;
        protected static readonly int defaltDelay = 20;
        protected int currentCount = 1;

        protected static IWebDriver browser;
        protected string _login, _pass, _text;
        protected int _timer;

        public BASEselenium(string login, string pass, string text, int timer)
        {
            this._login = login;
            this._pass = pass;
            this._text = text;
            if (timer <= 25)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                _timer = defaltDelay;
                Console.WriteLine("[WARNING] Delay is 20 sec. now!");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                this._timer = timer;
            }
        }
    }

    struct comLinks                             //this struct'll link group's params
    {
        private string GroupNum, PostNum;      //Every group contains 2 values 
        private string groupLink;             // number of group and number of last post

        public string grouplink
        {
            get { return groupLink; }
            private set { groupLink = value; }
        }
        public string groupnum                 //
        {
            get { return GroupNum; }
            private set { GroupNum = value; }
        }
        public string postnum                       // 
        {
            get { return PostNum; }
            private set { PostNum = value; }
        }
        public comLinks(string groupNum, string postNum, string grouplink)     //initialize struct
        {
            GroupNum = groupNum;
            PostNum = postNum;
            groupLink = grouplink;
        }
        public string getParamString()                       //We should get finish group+post number
        {                                                   //it'll allow to find our post 
            return GroupNum + "_" + PostNum;               //And we'll comment it
        }
        public string getParamString(int num)              //we can get modified number
        {                                                         //we'll remove last 2 digits and enter new 2 
            string buffer = postnum.Remove(postnum.Length-2);
            if (num < 10) {
                buffer += "0";
                buffer += num.ToString();
            }
            else
            {
                buffer += num.ToString();
            }
            return GroupNum+"_"+buffer;
        }
    }

    class seleniumHundler : BASEselenium              
    {
        //private int leftBorder, rightBorder;
        private int CompletedSum = 0;
        private Xmlelement[] links = null;
        private comLinks[] _comPage = null;   
        public seleniumHundler(string login, string pass, string text, int timer) : base(login,pass,text,timer)
        {
            browser = new OpenQA.Selenium.Chrome.ChromeDriver();
        }
        //initialize own constructor and launch Auth
        public seleniumHundler(string login, string pass, string text, int timer, Xmlelement[] vector) : base(login, pass, text, timer)
        {
            links = vector;
            _comPage = new comLinks[links.Length];
            browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            Auth();
            for (int i = 0; i < links.Length; i++)
            {                    //we'll navigate to link and get our number by previously shown method  
                _comPage[i] = returnParams(links[i].link);
                colorMes($"Group id: {_comPage[i].groupnum} post id: {_comPage[i].postnum} link: {_comPage[i].grouplink}",ConsoleColor.Yellow);
            }
            colorMes("Initializing was successfully completed", ConsoleColor.Green);
            comnavTo();               //launch method for navigate to url
        } //

        private void Auth()   //Auth method
        {
            try
            {
                browser.Navigate().GoToUrl("http://vk.com"); //navigate to vk.com site
                Thread.Sleep(100);
                IWebElement login = browser.FindElement(By.Id("index_email")); //find element from site code index_email
                Thread.Sleep(100);
                login.SendKeys(_login); //Enter text to element
                IWebElement Password = browser.FindElement(By.Id("index_pass")); //find element from site code index_pass (password)
                Password.SendKeys(_pass); //Enter text password to element
                IWebElement Login_button = browser.FindElement(By.Id("index_login_button")); //find element login btn
                Login_button.Click(); // press button "Log in"
                Console.WriteLine("AUTH WAS SUCCESSFULLY COMPLETED!");
                Thread.Sleep(1000); //
            }
            catch (Exception er)
            {
                colorMes("[ERROR] Uncorrected password or login - " + er.HelpLink, ConsoleColor.Red);
            }

        }
        private comLinks returnParams(string link) {
            try
            {
                browser.Navigate().GoToUrl(link);
                IWebElement element = browser.FindElements(By.ClassName("wall_post_cont"))[2];
                string k = element.GetAttribute("id");
                Console.WriteLine(k);
                return new comLinks( k.Remove(k.IndexOf("_")).Remove(0,4), k.Remove(0,k.IndexOf("_")+1) ,link);
                //wpt-[1..9]_[1..9] use mask to get number
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return new comLinks();
        }

        private bool comnavTo()     //finish method for spaming  
        {
            browser.Manage().Window.Maximize();
            for (int i = 0; i < _comPage.Length; i++)    //cycle will get comLinks objects and use their fields
            {
                int count = links[i].count;
                int postNumber = Convert.ToInt32(_comPage[i].postnum.Substring(_comPage[i].postnum.Length - 2)); 
                browser.Navigate().GoToUrl(_comPage[i].grouplink); //we navigate to site
                for (int j = postNumber; j > 1; j--)                //Every group contains a lot of posts
                {                                              //we will get each of them
                    try
                    {
                        Console.WriteLine(j);
                        Thread.Sleep(300);
                        colorMes("Start", ConsoleColor.Gray);
                        IWebElement comment = browser.FindElement(By.Id("reply_fakebox-" + _comPage[i].getParamString(j)));
                        if (comment.Displayed)                             //we try to find element frop site
                        {                                                   //then we will just enter our text and press button send
                            Actions actions = new Actions(browser);
                            actions.MoveToElement(comment);
                            actions.Click();
                            actions.SendKeys(_text);
                            actions.Build().Perform();
                            colorMes("We're entering the text...", ConsoleColor.Yellow);
                            IWebElement commentBTN = browser.FindElement(By.Id("reply_button-" + _comPage[i].getParamString(j)));
                            commentBTN.Click();
                            colorMes("Sending was successfully completed! ", ConsoleColor.Green);
                            CompletedSum++;
                            if (CompletedSum >= count)
                            {
                                CompletedSum = 0;
                                break;
                            }
                            Thread.Sleep(1000 * _timer);
                        }
                        colorMes("Exception to locale element //---//", ConsoleColor.Yellow);
                    }
                    catch
                    {
                        colorMes("[ERROR] Element was not found", ConsoleColor.DarkRed);
                    }
                }
            }
            browser.Close();
            return true;
        }
        private void colorMes(string mess, ConsoleColor color) {    //output messages with color;)
            Console.BackgroundColor = color;
            Console.WriteLine(mess);
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public bool navTo()               //spam to personal messages
        {
            browser.Manage().Window.Maximize();
                Auth();
                while (currentCount <= maxCount)
                {
                    try
                    {
                        browser.Navigate().GoToUrl(find());                 //generate new link
                        Thread.Sleep(50);
                        IWebElement writeBtn = browser.FindElement(By.PartialLinkText("Write message")); //find element from site code with write button
                         writeBtn.Click();                                  //then press it
                        Thread.Sleep(150);
                        IWebElement _massage = browser.FindElement(By.Id("mail_box_editable")); //find element from site code with text box
                        _massage.SendKeys(_text);                                       //enter our message
                        Thread.Sleep(1000);                                                 
                        IWebElement sendBtn = browser.FindElement(By.Id("mail_box_send")); //press send button
                        Thread.Sleep(100);
                        sendBtn.Click();
                    }
                    catch (Exception er)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] " + er.Message);
                        return false;
                    }
                    currentCount++;
                    Thread.Sleep(_timer * 1000);
                }
                browser.Close();
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("[WARNING] You have done your limit");
            return true;
        }
        private string find()              //function will generate link with random pers. id (for pm mode)
        {
            Random random = new Random();
            int ran_num = random.Next(700000);
            string name = "https://vk.com/id" + ran_num;
            return name;
        }
    }
}
//