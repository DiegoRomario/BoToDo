using System.Collections.Generic;

namespace ToDo.Bot
{
    public class User
    {
        public List<string> TasksList = new List<string>();
        public static string UserID { get; set; }
    }
}
