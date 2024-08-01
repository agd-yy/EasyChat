using Newtonsoft.Json;

namespace EasyChat.Extensions;

public static class JsonExtension
{
    public static string? Serialize(this object obj)
    {
        try
        {
            return JsonConvert.SerializeObject(obj);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static T? Deserialize<T>(this string content)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (Exception)
        {
            return default;
        }
    }
}