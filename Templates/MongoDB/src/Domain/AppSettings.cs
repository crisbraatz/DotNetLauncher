using System.Globalization;

namespace Domain;

public static class AppSettings
{
    public static string AppEnvironment =>
        Environment.GetEnvironmentVariable("APP_ENVIRONMENT")?.Trim().ToLowerInvariant() ?? "development";

    public static CultureInfo AppLanguage => new(Environment.GetEnvironmentVariable("APP_LANGUAGE")?.Trim() ?? "en-US");
    public static string AppName => Environment.GetEnvironmentVariable("APP_NAME")?.Trim() ?? "TemplateMongoDB";
    public static string AppVersion => Environment.GetEnvironmentVariable("APP_VERSION")?.Trim() ?? "1.0.0";
    public static bool IsDevelopment => AppEnvironment is "development";

    public static string JwtAudience =>
        Environment.GetEnvironmentVariable("JWT_AUDIENCE")?.Trim() ?? "DEFAULT_JWT_AUDIENCE";

    public static string JwtIssuer => Environment.GetEnvironmentVariable("JWT_ISSUER")?.Trim() ?? "DEFAULT_JWT_ISSUER";

    public static string JwtSecurityKey =>
        Environment.GetEnvironmentVariable("JWT_SECURITY_KEY")?.Trim() ?? "DEFAULT_256_BITS_JWT_SECURITY_KEY";

    public static string MongoDbConnectionString =>
        Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING")?.Trim() ??
        "mongodb://mongo:mongo@127.0.0.1:27017";

    public static string MongoDbDatabase =>
        Environment.GetEnvironmentVariable("MONGODB_DATABASE")?.Trim() ?? "TemplateMongoDB";

    public static string RabbitMqConnectionString =>
        Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")?.Trim() ??
        "amqp://guest:guest@127.0.0.1:5672/";
}