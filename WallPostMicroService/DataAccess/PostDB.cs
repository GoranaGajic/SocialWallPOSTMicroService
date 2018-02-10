using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using URISUtil.DataAccess;
using URISUtil.Logging;
using URISUtil.Response;
using WallPostMicroService.Models;

namespace WallPostMicroService.DataAccess
{
    public class PostDB
    {
        private static Post ReadRow(SqlDataReader reader)
        {
            Post retVal = new Post();

            retVal.Id = (Guid)reader["id"];
            retVal.DateCreated = (DateTime)reader["date_created"];
            retVal.Text = (string)reader["text"];
            retVal.Rating = (decimal)reader["rating"];
            retVal.Views = (int)reader["views"];
            retVal.Attachment = (string)reader["attachment"];
            retVal.Location = (string)reader["location"];
            retVal.Active = (bool)reader["active"];
            retVal.UserId = (Guid)reader["user_id"];

            return retVal;
        }

        private static string AllColumnSelect
        {
            get
            {
                return @"
                    [Post].[id],
	                [Post].[date_created],
	                [Post].[text],
                    [Post].[rating],
                    [Post].[views],
                    [Post].[attachment],
                    [Post].[location],
                    [Post].[active],
                    [Post].[user_id]
                ";
            }
        }

        private static void FillData(SqlCommand command, Post post)
        {
            command.AddParameter("@Text", SqlDbType.NVarChar, post.Text);
            command.AddParameter("@Rating", SqlDbType.Decimal, post.Rating);
            command.AddParameter("@Views", SqlDbType.Int, post.Views);
            command.AddParameter("@Attachment", SqlDbType.VarChar, post.Attachment);
            command.AddParameter("@Location", SqlDbType.NVarChar, post.Location);
            command.AddParameter("@Active", SqlDbType.Bit, post.Active);
            command.AddParameter("@UserId", SqlDbType.UniqueIdentifier, post.UserId);
        }

        public static List<Post> GetAllPosts(ActiveStatusEnum active)
        {
            try
            {
                List<Post> retVal = new List<Post>();
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT {0} FROM [post].[Post] 
                        WHERE @Active IS NULL OR [post].[Post].Active = @Active 
                        ORDER BY 
                        CONVERT(DateTime, date_created,101)  DESC
                        ", AllColumnSelect);

                    command.Parameters.Add("@Active", SqlDbType.Bit);
                    switch (active)
                    {
                        case ActiveStatusEnum.Active:
                            command.Parameters["@Active"].Value = true;
                            break;
                        case ActiveStatusEnum.Inactive:
                            command.Parameters["@Active"].Value = false;
                            break;
                        case ActiveStatusEnum.All:
                            command.Parameters["@Active"].Value = DBNull.Value;
                            break;
                    }
                    System.Diagnostics.Debug.WriteLine(command.CommandText);
                    connection.Open();

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadRow(reader));
                        }
                    }
                }
                return retVal;
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static List<Post> GetPostsByUserId(Guid id, ActiveStatusEnum active)
        {
            try
            {
                List<Post> retVal = new List<Post>();
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT {0} FROM [post].[Post] 
                        WHERE @Active IS NULL OR [post].[Post].Active = @Active 
                        AND [post].[Post].user_id = @UserId
                        ORDER BY 
                        CONVERT(DateTime, date_created,101)  DESC
                        ", AllColumnSelect);

                    command.Parameters.Add("@Active", SqlDbType.Bit);
                    command.AddParameter("@UserId", SqlDbType.UniqueIdentifier, id);
                    switch (active)
                    {
                        case ActiveStatusEnum.Active:
                            command.Parameters["@Active"].Value = true;
                            break;
                        case ActiveStatusEnum.Inactive:
                            command.Parameters["@Active"].Value = false;
                            break;
                        case ActiveStatusEnum.All:
                            command.Parameters["@Active"].Value = DBNull.Value;
                            break;
                    }
                    System.Diagnostics.Debug.WriteLine(command.CommandText);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadRow(reader));
                        }
                    }
                }
                return retVal;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static Post GetPost(Guid Id)
        {
            try
            {
                Post retVal = new Post();

                using(SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT {0} FROM [post].[Post] 
                        WHERE [id] = @Id;
                        UPDATE [post].[Post]
                        SET [post].[Post].[views] = [post].[Post].[views] + 1
                        WHERE [post].[Post].[id] = @Id;
                    ", AllColumnSelect);
                    command.AddParameter("@Id", SqlDbType.UniqueIdentifier, Id);

                    System.Diagnostics.Debug.WriteLine(command.CommandText);
                    connection.Open();

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = ReadRow(reader);
                        }
                        else
                        {
                            ErrorResponse.ErrorMessage(HttpStatusCode.NotFound);
                            return null;
                        }
                    }
                }
                return retVal;
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static Post InsertPost(Post post)
        {
             
            try
            {
                using(SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        INSERT INTO [post].[Post]
                        (
                            [date_created],
                            [text],
                            [rating],
                            [views],
                            [attachment],
                            [location],
                            [active],
                            [user_id]                
                        )
                        VALUES
                        (
                            GETDATE(),
                            @Text,
                            @Rating,
                            @Views,
                            @Attachment,
                            @Location,
                            @Active,
                            @UserId
                        );");
                    FillData(command, post);
                    if (post.Attachment == null)
                        if (post.Text == null || post.Text == "")
                            return null;
                    connection.Open();
                    command.ExecuteNonQuery();

                    return post;

                }
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static Post UpdatePost(Post post, Guid id)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE
                            [post].[Post]
                        SET
                            [text] = @Text,
                            [rating] = @Rating,
                            [views] = @Views,
                            [attachment] = @Attachment,
                            [location] = @Location,
                            [active] = @Active
                        WHERE
                            [Id] = @Id
                    ");
                    FillData(command, post);
                    command.AddParameter("@Id", SqlDbType.UniqueIdentifier, id);
                    if (post.Attachment == null)
                        if (post.Text == null || post.Text == "")
                            return null;
                    connection.Open();
                    command.ExecuteNonQuery();

                    return GetPost(id);
                }
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static void DeletePost(Guid id)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE
                            [post].[Post]
                        SET
                            [active] = 0
                        WHERE
                            [id] = @Id     
                    ");
                    command.AddParameter("@Id", SqlDbType.UniqueIdentifier, id);
                    connection.Open();
                    command.ExecuteNonQuery();

                }
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}