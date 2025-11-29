namespace Core.Data;

public static class StaticBookCollectionsData
{
    public static string[] StaticBookCollectionsNames =>
    [
        Favorite,
        WantToRead,
        Reading,
        Read
    ];

    public const string Favorite = "Избранное";
    public const string Reading = "Читаю";
    public const string Read = "Прочитано";
    public const string WantToRead = "Хочу прочитать";
}