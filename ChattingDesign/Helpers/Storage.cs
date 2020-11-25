using API.Models;
using System.Net.Http;

namespace ChattingDesign.Helpers
{
    public class Storage
    {
        private static Storage instance_;

        public static Storage Instance()
        {
            if (instance_ == null)
            {
                instance_ = new Storage();
            }
            return instance_;
        }

        public User CurrentUser { get; set; }
        public HttpClient APIClient { get; set; }
        public string EnvironmentPath { get; set; }
    }
}
