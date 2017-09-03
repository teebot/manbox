using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessBrowser
{
    public class Agent
    {
        public static string GetHtmlFromUrl(string url, string toReplace, string replaceValue)
        {
            using (PhantomJSDriver driver = new PhantomJSDriver())
            {
                driver.Url = url;
                
                var source = driver.PageSource.Replace(toReplace, replaceValue);

                return source;
            }
        }
    }
}
