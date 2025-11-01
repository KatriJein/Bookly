namespace Core.Options;

public class BooklyOptions
{
    public string DbConnectionString { get; set; }
    public string BooklyFilesStorageBucketName { get; set; }
    public string BucketServiceUrl { get; set; }
}