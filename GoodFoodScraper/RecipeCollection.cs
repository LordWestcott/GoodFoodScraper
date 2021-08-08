using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodFoodScraper
{
    public class RecipeCollection
    {
        public int rcid { get; set; }
        public string MasterCategoryLink { get; set; }
        public string Name { get; set; }

        public List<Category> SubCategories { get; set; } = new List<Category>();
 
    }

    public class Recipe
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string StarRating { get; set; }
        public string ReviewAmount { get; set; }
        public string CookTime { get; set; }
        public string Difficulty { get; set; }
        public string ShortDescription { get; set; }
    }

    public class Category
    {
        public int rcatid { get; set; }
        public int rcid { get; set; }
        public string CategoryName { get; set; }
        public string CategoryLink { get; set; }
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
        public DateTime LastScraped { get; set; }
    }
}
