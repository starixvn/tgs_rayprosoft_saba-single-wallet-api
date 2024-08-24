namespace SingleWallet.API;

public record MongoDBOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost";
}