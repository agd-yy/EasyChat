namespace EasyChat.ViewModel
{
    /// <summary>
    /// 用户相关(用户名修改，
    /// </summary>
    public class UserHandle : SingletonUtils<UserHandle>
    {
        private UserHandle()
        {
        }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ServiceIp { get; set; }
    }
}
