using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace pjPractice
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _opt = null;
        protected ChromeDriver _driver = null;

        string number, category, title, date, seeing, webAddress;
        Button[] buts = new Button[9];

        string row;
        string[] value = new string[7]; //number, category, title, date, seeing, webAddress, Isscrapped
        string[] AllString, ScrapString; //test.txt, scrap.txt

        int stringIndex = 1, scarpIndex = 1; //가장 마지막 == 제일 처음 공지사항을 가리킴

        int pageNumber = 1; //페이지 수 이동 체크
        int tempPage = 0; 

        public MainWindow()
        {
            InitializeComponent();

            leftButton.Click += Left_Click;
            rightButton.Click += Right_Click;

            buts[0] = notice1;
            buts[1] = notice2;
            buts[2] = notice3;
            buts[3] = notice4;
            buts[4] = notice5;
            buts[5] = notice6;
            buts[6] = notice7;
            buts[7] = notice8;
            buts[8] = notice9;

            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _opt = new ChromeOptions();
            _opt.AddArgument("disable-gpu");

            setStringvalue();
            setBut(stringIndex - 1);
        }

        private void SearchWeb(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = File.AppendText("test.txt");
            int index = 3; //3페이지 내에서만 검색, 숫자 바꾸면 됨

            _opt.AddArgument("headless");

            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"https://www.jbnu.ac.kr/kor/?menuID=139&pno={index}");
            var element = _driver.FindElement(By.XPath("//*[@id='print_area']/div[2]/table/tbody/tr[1]/th"));

            row = AllString[stringIndex - 1];
            value = row.Split('\t');

            int valueTemp, numberTemp;
            int.TryParse(value[0], out valueTemp);

            while (index > 0) { 
                _driver.Navigate().GoToUrl($"https://www.jbnu.ac.kr/kor/?menuID=139&pno={index}");
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            
                for (int i = 9; i > 0; i--)
                {
                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/th"));
                    number = element.Text;

                    if(int.TryParse(number, out numberTemp))
                    {
                        if (valueTemp >= numberTemp)
                            continue;
                    }

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[1]/span"));
                    category = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[2]/span/a"));
                    title = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[5]"));
                    date = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[6]"));
                    seeing = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[2]/span/a"));
                    webAddress = element.GetAttribute("href");

                    sw.WriteLine($"{number}\t{category}\t{title}\t{date}\t{seeing}\t{webAddress}\t0");
                }
                index--;
            }
            sw.Close();

            setStringvalue();
            setBut(stringIndex - 1);
        }

        private void setBut(int n)
        {
            for(int i = 0; i < 9; i++)
            {
                if (n < 0 || n > stringIndex - 1) {
                    return;
                }
                    
                row = AllString[n];
                value = row.Split('\t');
                buts[i].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";
                n--;
            }
            tempPage = n;
        }

        private void setStringvalue()
        {
            StreamReader sr = new StreamReader(new FileStream("test.txt", FileMode.Open));
            string temp = sr.ReadToEnd();
            stringIndex = temp.Split('\n').Length - 1;

            AllString = new string[stringIndex];
            AllString = temp.Split('\n');

            pageNumber = stringIndex;
            sr.Close();
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= AllString.Length - 1)
                return;

            pageNumber += 9;
            setBut(pageNumber);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber <= 0)
                return;
            pageNumber = tempPage;
            setBut(pageNumber);
        }

        private void Scarp_Click(object sender, RoutedEventArgs e) //추가 필요, 스크랩 창 따로 띄우기(스크롤)
        {
            StreamReader sr = new StreamReader(new FileStream("scrap.txt", FileMode.Open)); //scrap.txt 파일에 내용추가하는 함수 만들기
            string temp = sr.ReadToEnd();
            stringIndex = temp.Split('\n').Length - 1;

            ScrapString = new string[scarpIndex];
            ScrapString = temp.Split('\n');

            //따로 창 띄우기

            sr.Close();
        }


        private void Serach_Click(object sender, RoutedEventArgs e) //검색 -> 파일 내에서 검색? 웹 사이트에서 바로 검색?
        {

        }
    }
}
