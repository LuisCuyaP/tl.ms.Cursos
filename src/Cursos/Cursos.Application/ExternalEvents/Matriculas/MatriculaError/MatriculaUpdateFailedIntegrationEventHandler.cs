using Cursos.Domain.Cursos;
using MediatR;

namespace Cursos.Application.ExternalEvents.Matriculas.MatriculaError;
public class MatriculaUpdateFailedIntegrationEventHandler : INotificationHandler<MatriculaUpdateFailedIntegrationEvent>
{
    public readonly ICursoRepository _cursoRepository;

    public MatriculaUpdateFailedIntegrationEventHandler(ICursoRepository cursoRepository)
    {
        _cursoRepository = cursoRepository;
    }

    public async Task Handle(MatriculaUpdateFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var curso = await _cursoRepository.GetByIdAsync(notification.CursoId, cancellationToken);
        if (curso is null)
        {
            return;
        }

        curso.SumarCupo();
        await _cursoRepository.UpdateAsync(curso.Id, curso, cancellationToken);
    }
}
