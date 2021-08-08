using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;

namespace GoodFoodScraper
{
    public class Sqlhandler
    {
        public static void StageRecipeCollection(RecipeCollection collection)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.StageRecipeCollection", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CollectionName", collection.Name);
                    cmd.Parameters.AddWithValue("@CollectionLink", collection.MasterCategoryLink);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void MergeStagedRecipeCollection()
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.RecipeCollections_MergeStaged", cnx))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<RecipeCollection> SelectRecipeCollections()
        {
            var collections = new List<RecipeCollection>();
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.SelRecipeCollections", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return collections;
                    }

                    while (reader.Read())
                    {
                        var col = new RecipeCollection();
                        col.rcid = reader.GetInt32(0);
                        col.Name = reader.GetString(3);
                        col.MasterCategoryLink = reader.GetString(4);
                        collections.Add(col);
                    }
                    
                }
            }

            return collections;
        }

        public static void StageRecipeCategory(Category cat, int rcid)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.StageRecipeCatagory", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryName", cat.CategoryName);
                    cmd.Parameters.AddWithValue("@CategoryLink", cat.CategoryLink);
                    cmd.Parameters.AddWithValue("@rcid", rcid);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void MergeStagedRecipeCategories()
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.RecipeCategories_MergeStaged", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void MergeStagedRecipe()
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.Recipe_MergeStaged", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateCatagoryLastScraped(int rcatid)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.UpdCategoryLastScraped", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@rcatid", rcatid);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static List<Category> SelectTenCategories()
        {
            var categories = new List<Category>();
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.SelTenRecipeCatagories", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return categories;
                    }

                    while (reader.Read())
                    {
                        var col = new Category();
                        col.rcatid = reader.GetInt32(0);
                        col.rcid = reader.GetInt32(1);
                        col.CategoryName = reader.GetString(4);
                        col.CategoryLink = reader.GetString(5);
                        categories.Add(col);
                    }

                }
            }

            return categories;
        }

        public static void StageRecipe(Recipe recipe, int rcatid)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.AppSettings["SQLConnection"]))
            {
                cnx.Open();
                using (var cmd = new SqlCommand("bbc.stagerecipe", cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@rcatid", rcatid);
                    cmd.Parameters.AddWithValue("@RecipeName", recipe.Name);
                    cmd.Parameters.AddWithValue("@RecipeLink", recipe.Link);
                    //TODO capture these and convert to correct datatype.
                    //cmd.Parameters.AddWithValue("@StarRating", recipe.StarRating);
                    //cmd.Parameters.AddWithValue("@ReviewAmount", recipe.ReviewAmount);
                    //cmd.Parameters.AddWithValue("@CookTime", recipe.CookTime);
                    //cmd.Parameters.AddWithValue("@Difficulty", recipe.Difficulty);
                    cmd.Parameters.AddWithValue("@ShortDescription", recipe.ShortDescription);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }


}
