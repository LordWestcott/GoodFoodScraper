using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace GoodFoodScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Scrape();
        }

        public static void Scrape()
        {
            FirefoxOptions options = new FirefoxOptions();
            using (IWebDriver driver = new FirefoxDriver(options))
            {
                //get category links.
                
                IWebElement nextButton = null;
                
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl(@"https://www.bbcgoodfood.com/recipes/category");

                ////update categories. - PART 1.
                //var MainCategories = GetCategoryLinks(driver);
                //foreach (var cat in MainCategories)
                //{
                //    Sqlhandler.StageRecipeCollection(new RecipeCollection()
                //    {
                //        MasterCategoryLink = cat.CategoryLink,
                //        Name = cat.CategoryName
                //    });
                //}
                //Sqlhandler.MergeStagedRecipeCollection();

                ////Part 2
                //var RecipeCollections = Sqlhandler.SelectRecipeCollections();
                //GetSubCollections_UpdateSQL(RecipeCollections, driver);

                //Part 3
                var tenCategories = Sqlhandler.SelectTenCategories();
                GetRecipeCards(tenCategories, driver);


            }
        }

        private static void GetRecipeCards(List<Category> MasterCategories, IWebDriver driver)
        {
            IWebElement nextButton;
            foreach (var cat in MasterCategories)
            {
                    driver.Navigate().GoToUrl(cat.CategoryLink);

                    var stop = false;
                    do
                    {
                        var nextbutton = FindNextButton(driver, ref stop);

                        var RecipeCards = driver.FindElements(By.ClassName("standard-card-new"));

                        foreach (var card in RecipeCards)
                        {
                            var recipe = new Recipe();
                            recipe.Name = card.FindElement(By.ClassName("standard-card-new__article-title")).Text;
                            recipe.Link = card.FindElement(By.ClassName("qa-card-link"))
                                .GetAttribute("href");
                            recipe.ShortDescription = card.FindElement(By.ClassName("body-copy-small")).Text;
                            //recipe.StarRating = card.FindElement(By.ClassName("sr-only")).Text;
                            //recipe.ReviewAmount = card.FindElement(By.ClassName("ratings-stars__reactions-value")).Text;

                            Sqlhandler.StageRecipe(recipe, cat.rcatid);
                        }

                        if (!stop)
                        {
                            try
                            {
                                nextbutton.Click();
                            }
                            catch (Exception e)
                            {
                                driver.Navigate().Refresh();
                                try
                                {
                                    var reattempt = FindNextButton(driver, ref stop);
                                    reattempt.Click();
                                }
                                catch (Exception e2)
                                {
                                    stop = true;
                                }
                            }
                        }
                    } while (!stop);
                
                Sqlhandler.MergeStagedRecipe();
                Sqlhandler.UpdateCatagoryLastScraped(cat.rcatid);
            }
        }

        private static void GetSubCollections_UpdateSQL(List<RecipeCollection> collections, IWebDriver driver)
        {

            foreach (var col in collections)
            {
                driver.Navigate().GoToUrl(col.MasterCategoryLink);
                var subCategories = GetCategoryLinks(driver);
                foreach (var cat in subCategories)
                {
                    Sqlhandler.StageRecipeCategory(cat, col.rcid);
                }
                Sqlhandler.MergeStagedRecipeCategories();
            }
        }


        private static List<Category> GetCategoryLinks(IWebDriver driver)
        {
            IWebElement nextButton;
            var stop = false;
            List<Category> CategoryLink = new List<Category>();
            do
            {
                nextButton = FindNextButton(driver, ref stop);
                ReadOnlyCollection<IWebElement> cats;
                try
                {
                    cats = driver.FindElements(By.ClassName("qa-card-link"));
                }
                catch (Exception e)
                {
                    continue;
                }

                foreach (var cat in cats)
                {
                    try
                    {
                        CategoryLink.Add(new Category()
                        {
                            CategoryLink = cat.GetAttribute("href"),
                            CategoryName = cat.Text
                        });
                    }
                    catch (Exception e){}
                }

                if (!stop)
                {
                    try
                    {
                        var agreebutton = driver.FindElement(By.ClassName("css-1x23ujx"));
                        agreebutton.Click();
                    }
                    catch (Exception e)
                    {
                    }

                    nextButton.Click();
                }
            } while (!stop);

            return CategoryLink;
        }

        private static IWebElement FindNextButton(IWebDriver driver, ref bool stop)
        {
            IWebElement nextButton;
            try
            {
                nextButton = driver.FindElement(By.ClassName("pagination-arrow__icon--next"));
            }
            catch (Exception)
            {
                nextButton = null;
                stop = true;
            }

            return nextButton;
        }
    }
}
