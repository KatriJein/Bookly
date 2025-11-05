namespace Core;

public static class Const
{
    public const string YcStaticKeyIdentifier = "YC_STATIC_KEY_IDENTIFIER";
    public const string YcStaticKey = "YC_STATIC_KEY";
    public const string GoogleBooksApiKey = "GOOGLE_BOOKS_API_KEY";
    public static HashSet<string> SupportedAvatarFileExtensions => [".jpg", ".png", ".gif", ".jpeg"];
    
    public static string BooksApiScrapingJob => "BooksApiScrapingJob";
}