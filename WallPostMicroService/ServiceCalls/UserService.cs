using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using URISUtil;
using WallPostMicroService.Models;

namespace WallPostMicroService.ServiceCalls
{
    public class UserService
    {
        public static User GetUser(Guid userId)
        {
            User user;
            string queryPart = "/api/User/" + userId.ToString();
            Uri serviceUrl = new Uri(UrlUtil.GetServiceUrl("User", "User"), queryPart);

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serviceUrl))
                {
                    using (HttpResponseMessage response = client.SendAsync(request, CancellationToken.None).Result)
                    {
                        user = response.Content.ReadAsAsync<User>().Result;
                    }
                }
            }
            return user;
        }
    }
}