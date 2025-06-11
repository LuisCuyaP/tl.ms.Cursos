using Cursos.Domain.Cursos;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Cursos.Infrastructure.Serializers;

public class CapacidadCursoSerializer : SerializerBase<CapacidadCurso>
{
    public override CapacidadCurso Deserialize
    (BsonDeserializationContext context,
    BsonDeserializationArgs args
     )
    {
       return new CapacidadCurso(context.Reader.ReadInt32());
    }

    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        CapacidadCurso capacidad)
    {
        context.Writer.WriteInt32(capacidad.Value);
    }
}