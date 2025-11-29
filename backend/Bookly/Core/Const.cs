namespace Core;

public static class Const
{
    public const string YcStaticKeyIdentifier = "YC_STATIC_KEY_IDENTIFIER";
    public const string YcStaticKey = "YC_STATIC_KEY";
    public const string GoogleBooksApiKey = "GOOGLE_BOOKS_API_KEY";
    public const string JwtSecretKey =  "JWT_SECRET_KEY";
    public static HashSet<string> SupportedAvatarFileExtensions => [".jpg", ".png", ".gif", ".jpeg"];
    
    public static string BooksApiScrapingJob => "BooksApiScrapingJob";

    public const int TrustedRatingsCount = 10;
    public const int ShortBookMaxPagesCount = 200;
    public const int MediumBookMaxPagesCount = 500;
    public const int LongBookMaxPagesCount = 800;

    public const int AbsolutelyHatedGenreWeight = -1;
}