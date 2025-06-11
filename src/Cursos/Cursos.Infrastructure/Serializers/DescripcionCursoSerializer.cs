using Cursos.Domain.Cursos;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Cursos.Infrastructure.Serializers;

public class DescripcionCursoSerializer : SerializerBase<DescripcionCurso>
{
    public override DescripcionCurso Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args)
    {
        return new DescripcionCurso(context.Reader.ReadString());
    }

    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        DescripcionCurso descripcion)
    {
        context.Writer.WriteString(descripcion.Value);
    }   
}