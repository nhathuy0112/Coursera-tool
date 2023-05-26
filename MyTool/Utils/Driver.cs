using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MyTool.Utils
{
    public class Driver
    {
        private readonly ChromeDriver _driver;
        private WebDriverWait _waiter;
        private readonly string _loginUrl = $"https://www.coursera.org/?authMode=login";
        private readonly Mode _mode;
        private readonly string _subjectName;
        public List<string> Links { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public Driver(ChromeDriver driver, AccountInfo accountInfo, List<string> links, Mode mode, string subjectName)
        {
            _driver = driver;
            AccountInfo = accountInfo;
            Links = links;
            _mode = mode;
            _subjectName = subjectName;
        }

        private void Login() 
        {
                _driver.Navigate().GoToUrl(_loginUrl);
                SetTimeToWait(300);
                var emailInput = _waiter
                    .Until(e => Find(By.CssSelector("input[placeholder=\"name@email.com\"]")));
                var passwordInput = _waiter
                    .Until(e => Find(By.CssSelector("input[placeholder=\"Enter your password\"]")));
                var loginButton = _waiter
                    .Until(e => Find(By.CssSelector("button[data-track-component=\"login_form_submit_button\"]")));
            
                emailInput.SendKeys(AccountInfo.Email);
                passwordInput.SendKeys(AccountInfo.Password);
                loginButton.Click();

                SetTimeToWait(300);
                try
                {
                    var check = _waiter
                        .Until(e => _driver.FindElement(By.CssSelector("p[data-e2e=\"UserPortraitFullName\"]")));
                }
                catch (NoSuchElementException e)
                {
                    throw new NoSuchElementException();
                }
        }

        private void GivePointAndSubmit()
        {
            
                SetTimeToWait(300);
                IWebElement bigForm = null;
                try
                {
                    bigForm = _waiter.Until(e => Find(By.CssSelector(".rc-SubmittedSubmissionView")));
                }
                catch (NoSuchElementException e)
                {
                    throw new NoSuchElementException();
                }
                
                var pointForms = FindAllFrom(bigForm, By.CssSelector(".rc-OptionsFormPart"));
                foreach (var form in pointForms)
                {
                    var pointButton = FindAllFrom(form, By.CssSelector(".option-input"));
                    if (pointButton[^1].Enabled)
                    {
                        pointButton[^1].Click();
                    }
                    else
                    {
                        break;
                    }
                }

                var commentAreas = FindAllFrom(bigForm, By.CssSelector(".c-peer-review-submit-textarea-field"));
                if (commentAreas.Any())
                {
                    foreach (var comment in commentAreas)
                    {
                        if (comment.Enabled)
                        {
                            comment.SendKeys("z");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                try
                {
                    var submitButton = FindFrom(bigForm, By.CssSelector(".rc-FormSubmit button")); 
                    submitButton.Click();
                }
                catch (NoSuchElementException e)
                {
                    throw new NoSuchElementException();
                }
        }
        
        private async Task StartReviewing()
        {
            int count = 0;
            try
            {
                Login();
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine("Kiểm tra login");
            }

            var listTask = new List<Task>();
            foreach (var reviewLink in Links)
            {
                listTask.Add(Task.Run(() =>
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript($"window.open('{reviewLink}');");
                }));
                await Task.WhenAll(listTask);
            }
            var tabs = _driver.WindowHandles;
            SetTimeToWait(300);
            for (var i = 1; i < tabs.Count; i++)
            {
                _driver.SwitchTo().Window(tabs[i]);
                try
                {
                    _waiter.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                    var continueButton = _waiter
                        .Until(e =>
                        {
                            Find(By.CssSelector("button[data-test=\"continue-button\"]")).Click();
                            return true;
                        });
                }
                catch (NoSuchElementException e)
                {
                    //ignore
                }
                catch (WebDriverTimeoutException e)
                {
                    //ignore
                }
                finally
                {
                    try
                    {
                        GivePointAndSubmit();
                        _driver.Close();
                        Console.WriteLine($"Xong link {++count}");
                    }
                    catch (NoSuchElementException e)
                    {
                        Console.WriteLine($"Đã chấm link {++count}");
                    }
                    finally
                    {
                        SetTimeToWait(0);
                    }
                }
            }
            _driver.Quit();
        }
        
        private async Task StartGettingLinks()
        {
            int count = 0;
            try
            {
                Login();
                var reviewLinks = new List<string>();
                var listTasks = new List<Task>();
                foreach (var submitLink in Links)
                {
                    listTasks.Add(Task.Run(() =>
                    {
                        ((IJavaScriptExecutor)_driver).ExecuteScript($"window.open('{submitLink}');");
                    }));
                }

                await Task.WhenAll(listTasks);
                var tabs = _driver.WindowHandles;
                SetTimeToWait(300);
                for (var i = 1; i < tabs.Count; i++)
                {
                    _driver.SwitchTo().Window(tabs[i]);
                    try
                    {
                        _waiter.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                        var continueButton = _waiter
                            .Until(e =>
                            {
                                Find(By.CssSelector("button[data-test=\"continue-button\"]")).Click();
                                return true;
                            });
                    }
                    catch (NoSuchElementException e)
                    {
                        
                    }
                    catch (WebDriverTimeoutException e)
                    {

                    }
                    finally
                    {
                        // NavigateToLink(submitLink);
                        var baseReviewlink = _driver.Url.Replace("submit", "review/");
                        SetTimeToWait(300);
                        var commentInput = _waiter.Until(e => Find(By.ClassName("_10nd10j")));
                        var id = commentInput.GetAttribute("id").Replace("~comment-input", "");
                        var reviewLink = baseReviewlink + id;
                        Console.WriteLine($"Đã lấy link {++count}");
                        reviewLinks.Add(reviewLink);
                        _driver.Close();
                        SetTimeToWait(0);
                    }
                }

                await Reader.WriteSubmitLinksToReviewLinks(reviewLinks, AccountInfo.Name, _subjectName);
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine("Kiểm tra login");
            }
            finally
            {
                _driver.Quit();
            }
        }

        public async Task Start()
        {
            if (_mode == Mode.GET)
            {
                await StartGettingLinks();
            }
            else
            { 
                await StartReviewing();
            }
        }

        private IWebElement Find(By selector)
        {
            return _driver.FindElement(selector);
        }

        private ReadOnlyCollection<IWebElement> FindAll(By selector)
        {
            return _driver.FindElements(selector);
        }

        private IWebElement FindFrom(IWebElement element, By selector)
        {
            return element.FindElement(selector);
        }

        private ReadOnlyCollection<IWebElement> FindAllFrom(IWebElement element, By selector)
        {
            return element.FindElements(selector);
        }

        private void SetTimeToWait(double second)
        {
            if (_waiter is null)
            {
                _waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(second));
            }
            else
            {
                _waiter.Timeout = TimeSpan.FromSeconds(second);
            }
        }

    }
}