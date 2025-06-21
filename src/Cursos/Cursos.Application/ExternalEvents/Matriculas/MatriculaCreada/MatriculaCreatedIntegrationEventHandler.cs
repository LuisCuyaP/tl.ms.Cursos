using Cursos.Application.Cursos.Events;
using Cursos.Application.Services;
using Cursos.Domain.Cursos;
using MediatR;

namespace Cursos.Application.ExternalEvents.Matriculas.MatriculaCreada;
internal sealed class MatriculaCreatedIntegrationEventHandler : INotificationHandler<MatriculaCreatedIntegrationEvent>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly IEventBus _eventBus;

    public MatriculaCreatedIntegrationEventHandler(ICursoRepository cursoRepository, IEventBus eventBus)
    {
        _cursoRepository = cursoRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(MatriculaCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var curso = _cursoRepository.GetByIdAsync(notification.CursoId);
        if (curso is null)
        {            
            return;
        }

        if(!curso.Result.TieneCupoDisponible())
        {
            _eventBus.Publish(new CursoSinCupoDisponibleIntegrationEvent(notification.MatriculaId));
            return;
        }

        curso.Result.RestarCupo();
        await _cursoRepository.UpdateAsync(curso.Result.Id, curso.Result, cancellationToken);
        _eventBus.Publish(new CursoConCupoDisponibleIntegrationEvent(notification.MatriculaId));
    }
}
