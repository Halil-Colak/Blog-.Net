namespace Blog.Helper;
public struct AppConfig
{
#if DEBUG
    public static string WebUrl => "https://localhost:44318/";
    public static string WebAPIUrl => "https://localhost:44317";

#else
    public static string WebUrl => "https://hbpanel.hctheme.com/";
    public static string WebAPIUrl => "https://hbapi.hctheme.com/";

#endif
}