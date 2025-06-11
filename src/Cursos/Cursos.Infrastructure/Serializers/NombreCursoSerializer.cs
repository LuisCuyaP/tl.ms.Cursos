using Cursos.Domain.Cursos;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Cursos.Infrastructure.Serializers;

public class NombreCursoSerializer : SerializerBase<NombreCurso>
{
    public override NombreCurso Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args)
    {
        return new NombreCurso(context.Reader.ReadString());
    }

    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        NombreCurso nombre)
    {
        context.Writer.WriteString(nombre.Value);
    }
    
}