using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using URISUtil.DataAccess;
using WallPostMicroService.DataAccess;
using WallPostMicroService.Models;

namespace WallPostMicroService.Controllers
{
    [RoutePrefix("api/Post")]
    public class PostController : ApiController
    {
        [Route(""), HttpGet]
        public IEnumerable<Post> GetAllPosts([FromUri]ActiveStatusEnum active = ActiveStatusEnum.Active)
        {
            return PostDB.GetAllPosts(active);
        }

        [Route("{id}"), HttpGet]
        public Post GetPost(Guid id)
        {
            return PostDB.GetPost(id);
        }

        [Route(""), HttpPost]
        public Post InsertPost(Post post)
        {
            return PostDB.InsertPost(post);
        }

        [Route("{id}"), HttpPut]
        public Post UpdatePost([FromBody]Post post, Guid id)
        {
            return PostDB.UpdatePost(post, id);
        }

        [Route("{id}"), HttpDelete]
        public void DeletePost(Guid id)
        {
            PostDB.DeletePost(id);
        }
    }
}