using API.Models;

namespace ChattingDesign.Helpers
{
    public class Storage
    {
        public static Storage instance_;

        public static Storage Instance()
        {
            if (instance_ == null)
            {
                instance_ = new Storage();
            }
            return instance_;
        }

        public User CurrentUser { get; set; }
    }
}
