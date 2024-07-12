using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace albertWebScraper;
class Program
{
    static ChromeDriver driver;
    static void Main(string[] args)
    {
        
        driver = new ChromeDriver();
        string[][] brandInfo = GetBrandNameAndID();
        foreach (string[] brand in brandInfo)
        {
            GetDoorstyleNameAndID(brand);
            //break; //remove
        }
        Console.ReadLine();

        //
    }

    public static string[] [] GetBrandNameAndID()
    {
        driver.Navigate().GoToUrl("https://www.searspartsdirect.com/buying-guide/water-filter-finder");
        var elements = driver.FindElements(By.ClassName("finderOption"));
        string[][] jaggedArray = new string[elements.Count][];
        int index = 0;
        foreach (IWebElement brandButton in elements)
        {
            string imageSrc = brandButton.FindElement(By.TagName("img")).GetAttribute("src");
            string searchString = "pd-brand-";
            string pattern = $"{Regex.Escape(searchString)}(.{{4}})";
            Match match = Regex.Match(imageSrc, pattern);
            string brandID = match.Groups[1].Value;
            jaggedArray[index] = new string[] { brandButton.FindElement(By.TagName("img")).GetAttribute("alt"), brandID};
            index++;
        }
        return jaggedArray;
        
    }

    public static void GetDoorstyleNameAndID(string[] brandNameAndID)
    {
        //
        string brandID = brandNameAndID[1];
        driver.Navigate().GoToUrl($"https://www.searspartsdirect.com/buying-guide/water-filter-finder?brandid={brandID}");
        var elements = driver.FindElements(By.ClassName("col"));
        var doorstyles = elements.Where(element => element.GetAttribute("src") != null).ToList();

        string[] IDs = new string[doorstyles.Count];
        string[] names = new string[doorstyles.Count];
        int index = 0;
        foreach (IWebElement element in doorstyles)
        {
            string imageSrc = element.GetAttribute("src");
            string pattern = @"pd-wf-(.*?)-closed";
            Match match = Regex.Match(imageSrc, pattern);
            IDs[index] = match.Groups[1].Value;
            names [index] = element.GetAttribute("alt").Replace(" logo", ""); 
            index++;
        }

        string brandName = brandNameAndID[0].Replace(" logo", ""); 
        foreach (string id in IDs)
        {
            driver.Navigate().GoToUrl($"https://www.searspartsdirect.com/buying-guide/water-filter-finder?brandid={brandID}&wfrefstyle={id}-closed");
            var locationElements = driver.FindElements(By.ClassName("col"));
            var locations = elements.Where(element => element.GetAttribute("src") != null).ToList();
            foreach(IWebElement location in locations)
            {
                Console.WriteLine(brandName + " " + id + " " +location.GetAttribute("alt").Replace(" logo", ""));
            }
        }

    }
}



