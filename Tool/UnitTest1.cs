using System;
using System.Diagnostics;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Tool
{
    public class Tests
    {
        protected IWebDriver driver;

        [SetUp]
        public void CreateDriver()
        {
            driver = new ChromeDriver();
        }

        [Test]
        public void ChromeSession()
        {
            driver.Navigate().GoToUrl("https://www.coursera.org/?authMode=login");
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var emailInput = waiter.Until((e) => driver.FindElement(By.CssSelector("input[placeholder=\"name@email.com\"]")));
            var passwordInput = waiter.Until(e => driver.FindElement(By.CssSelector("input[placeholder=\"Enter your password\"]")));
            var loginButton = waiter.Until(e => driver.FindElement(By.CssSelector("button[data-track-component=\"login_form_submit_button\"]")));
            emailInput.SendKeys("phunhqde150171@fpt.edu.vn");
            passwordInput.SendKeys("mn55y8kv");
            loginButton.Click();
            var check = new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until((e) => driver.FindElement(By.CssSelector("p[data-e2e=\"UserPortraitFullName\"]")).Displayed);
            if (check)
            {
                var linkSubmit =
                    "https://www.coursera.org/learn/introtoux-principles-and-processes/peer/TOn8V/10-000-floor-elevator/submit";
                // driver.Navigate().GoToUrl("https://www.coursera.org/learn/applying-project-management/peer/qVKiz/activity-draft-an-executive-summary/review/WjBexD2pEe2jpxLNszTtWw");
                driver.Navigate().GoToUrl(linkSubmit);
                var continueButton = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                    .Until(e => driver.FindElement(By.CssSelector("button[data-test=\"continue-button\"]")));
                continueButton?.Click();
                
                var textArea = new WebDriverWait(driver, TimeSpan.FromSeconds(15))
                    .Until(e => driver.FindElement(By.ClassName("_10nd10j")));

                string id = textArea.GetAttribute("id");

                Assert.AreEqual(id.Replace("~comment-input", ""), "b0g5WErfEe2vDA50Q1Ai9Q");
                
                // try
                // {
                //     var warningElement = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                //         .Until(e => driver.FindElement(By.CssSelector(".rc-AssignmentSubmitBeforeReviewWarning")));
                //     driver.Quit();
                // }
                // catch (Exception e)
                // {
                //     throw new NoSuchElementException();
                // }
                // var bigForm = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                //     .Until(e => driver.FindElement(By.CssSelector(".rc-SubmittedSubmissionView")));
                // var optionForms = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                //     .Until((e) => bigForm.FindElements(By.CssSelector(".rc-OptionsFormPart")));
                // foreach (var optionForm in optionForms)
                // {
                //     var optionInputButtons = optionForm.FindElements(By.CssSelector(".option-input"));
                //     if (optionInputButtons[^1].Enabled)
                //     {
                //         optionInputButtons[^1].Click();
                //     }
                //     else
                //     {
                //         break;
                //     }
                // }
                //
                // var textAreas = bigForm.FindElements(By.CssSelector(".c-peer-review-submit-textarea-field"));
                // foreach (var area in textAreas)
                // {
                //     if (area.Enabled)
                //     {
                //         area.SendKeys("z");
                //     }
                //     else
                //     {
                //         break;
                //     }
                // }
                //
                // IWebElement submitButton = null;
                // try
                // {
                //     submitButton = bigForm.FindElement(By.CssSelector(".rc-FormSubmit button"));
                // }
                // catch (NoSuchElementException e)
                // {
                // }
                // finally
                // {
                //     submitButton?.Click();
                // }
            }
            // driver.Quit();
        }
    }
}