namespace Cursos.Infrastructure;

public record MongoDbsettings
(
    string? ConnectionString,
    string? DatabaseName
);