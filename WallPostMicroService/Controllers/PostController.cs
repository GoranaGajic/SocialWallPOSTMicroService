using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using URISUtil.DataAccess;
using WallPostMicroService.DataAccess;
using WallPostMicroService.Models;
using WallPostMicroService.ServiceCalls;

namespace WallPostMicroService.Controllers
{
    [RoutePrefix("api")]
    public class PostController : ApiController
    {
        [Route("Post"), HttpGet]
        public IEnumerable<Post> GetAllPosts([FromUri]ActiveStatusEnum active = ActiveStatusEnum.Active)
        {
            return PostDB.GetAllPosts(active);
        }

        [Route("UserPosts/{id}"), HttpGet]
        public IEnumerable<Post> GetPostsByUserId(Guid id, [FromUri]ActiveStatusEnum active = ActiveStatusEnum.Active)
        {
            return PostDB.GetPostsByUserId(id, active);
        }

        [Route("Post/{id}"), HttpGet]
        public Post GetPost(Guid id)
        {
            return PostDB.GetPost(id);
        }

        [Route("Post"), HttpPost]
        public Post InsertPost(Post post)
        {
            return PostDB.InsertPost(post);
        }

        [Route("Post/{id}"), HttpPut]
        public Post UpdatePost([FromBody]Post post, Guid id)
        {
            return PostDB.UpdatePost(post, id);
        }

        [Route("Post/{id}"), HttpDelete]
        public void DeletePost(Guid id)
        {
            PostDB.DeletePost(id);
        }

        [Route("User/{id}"), HttpGet]
        public User GetUser(Guid id)
        {
            return UserService.GetUser(id);
        }
    }
}