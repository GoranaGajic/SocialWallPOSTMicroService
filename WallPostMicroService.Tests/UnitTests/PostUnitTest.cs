using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using NUnit.Framework;
using URISUtil.DataAccess;
using WallPostMicroService.DataAccess;
using WallPostMicroService.Models;

namespace WallPostMicroService.Tests.UnitTests
{
    
    public class PostUnitTest
    {
        ActiveStatusEnum active = ActiveStatusEnum.Active;
        Guid postId;
        SqlConnection connection;

        [SetUp]
        public void TestInitialize()
        {
            FileInfo file = new FileInfo("C:\\Users\\Gligoric\\Documents\\URIS\\postMSTestsInsert.sql");
            string script = file.OpenText().ReadToEnd();
            connection = new SqlConnection(DBFunctions.ConnectionString);
            
            SqlCommand command = connection.CreateCommand();
            command.CommandText = script;
            connection.Open();
            command.ExecuteNonQuery();
        }



        [Test]
        public void GetAllPostsSuccess()
        {
            List<Post> posts = PostDB.GetAllPosts(active);
            postId = posts[0].Id;
            Assert.AreEqual(4, posts.Count);

        }

        [Test]
        public void GetOnePostSuccess()
        {
            Guid id = PostDB.GetAllPosts(active)[0].Id;
            Post post = PostDB.GetPost(id);
            Assert.IsNotNull(post);
        }

        [Test]
        public void GetOnePostFailed()
        {
            Post post = PostDB.GetPost(Guid.NewGuid());
            Assert.IsNull(post);
        }

        [Test]
        public void InsertPostSuccess()
        {
            List<Post> posts = PostDB.GetAllPosts(active);
            Post post = new Post
            {
                Text = "new post text",
                Rating = 2.5m,
                Views = 10,
                Attachment = "attt",
                Location = "Novi Sad",
                Active = true,
                UserId = postId = posts[0].UserId
        };
            int oldNumberOfPosts = PostDB.GetAllPosts(active).Count;
            PostDB.InsertPost(post);
            Assert.AreEqual(oldNumberOfPosts + 1, PostDB.GetAllPosts(active).Count);
        }

        [Test]
        public void InsertPostFailed()
        {

            Post post = new Post
            {
                Text = "",
                Rating = 2.5m,
                Views = 10,
                Location = "Novi Sad",
                Active = true,
                UserId = postId = Guid.NewGuid()
            };
            int oldNumberOfPosts = PostDB.GetAllPosts(active).Count;
            PostDB.InsertPost(post);
            Assert.AreEqual(oldNumberOfPosts, PostDB.GetAllPosts(active).Count);
        }

        [Test]
        public void UpdatePostSuccess()
        {
            Guid id = PostDB.GetAllPosts(active)[0].Id;
            Post post = new Post
            {
                Text = "UpdateText",
                Rating = 3.5m,
                Views = 12,
                Attachment = "sadasd",
                Location = "Update Location",
                Active = true
            };
            Post updatedPost = PostDB.UpdatePost(post, id);
            Assert.AreEqual(updatedPost.Text, post.Text);
            Assert.AreEqual(updatedPost.Rating, post.Rating);
            Assert.AreEqual(updatedPost.Views, post.Views);
            Assert.AreEqual(updatedPost.Attachment, post.Attachment);
            Assert.AreEqual(updatedPost.Location, post.Location);
            Assert.AreEqual(updatedPost.Active, post.Active);
        }

        [Test]
        public void UpdatePostFailed()
        {
            Guid id = PostDB.GetAllPosts(active)[0].Id;
            Post post = new Post
            {
                Text = "",
                Rating = 3.5m,
                Views = 12,
                Attachment = null,
                Location = "Another update",
                Active = true,
                UserId = postId = Guid.NewGuid()
            };
            Post updatedPost = PostDB.UpdatePost(post, id);
            Assert.IsNull(updatedPost);
        }

        [Test]
        public void DeletePostSuccess()
        {
            Guid id = PostDB.GetAllPosts(active)[0].Id;
            PostDB.DeletePost(id);
            Assert.AreEqual(PostDB.GetPost(id).Active,false);
        }

        [Test]
        public void DeletePostFailed()
        {
            int numberOfOldPosts = PostDB.GetAllPosts(active).Count;
            PostDB.DeletePost(Guid.NewGuid());
            Assert.AreEqual(numberOfOldPosts, PostDB.GetAllPosts(active).Count);
        }

        [TearDown]
        public void TestCleanup()
        {
        
            SqlCommand command = connection.CreateCommand();
            command.CommandText = String.Format(@"DROP TABLE [post].[Post]");
            command.ExecuteNonQuery();
            connection.Close();
            
        }

    }
}
