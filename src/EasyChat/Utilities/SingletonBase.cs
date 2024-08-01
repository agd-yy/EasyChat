namespace EasyChat.Utilities;

/// <summary>
///     单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : class, new()
{
    private static readonly Lazy<T> Source = new(() => (T)Activator.CreateInstance(typeof(T), true)!, true);

    public static T Instance => Source.Value;
}

/// <summary>
///     可继承的单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonBase<T> where T : class
{
    private static readonly Lazy<T> Source = new(() => (T)Activator.CreateInstance(typeof(T), true)!, true);

    public static T Instance => Source.Value;
}