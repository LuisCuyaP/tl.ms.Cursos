ms Estudiantes(necesita url rabbit, bd pg, redis)
ms Cursos (necesita url rabbit y bd mongo)
ms Usuarios (necesita bd pg)
ms Docentes (necesita bd pg y redis)


/******** INCICIO FLUJO VS ********/

1. ms estudiantes / CrearMatriculaDomainEventHandler.cs /  1. flujo : cursos escuchara este publish y llegara a cursos/externalEvent/MatriculaCreatedIntegrationEventHandler 
2. ms cursos / MatriculaCreatedIntegrationEventHandler  // 2.  flujo: llega a este handler por el publish de ms.estudiantes/CrearMatriculaDomainEventHandler, y hago otro publish para enviarlo a ms.estudiantes  exactamente a CursoConCupoDisponibleIntegrationEventHandler
3. ms estudiantes / CursoConCupoDisponibleIntegrationEventHandler / 3. flujo: llega a este handler por el publish de ms.cursos/MatriculaCreatedIntegrationEventHandler y si es el flujo ideal sin error termina o si existe error crear publish y lo que hara es enviarlo a ms.cursos/MatriculaUpdateFailedIntegrationEventHandler


/******** FIN FLUJO VS ********/


*********** INICIO  FLUJO CREAR MATRICULA **************
(la escucha del evento de integracion CrearMatriculaDomainEventHandler lo tendra ms.Cursos)
1.EndPoint API CrearMatriculaController
2.CrearMatriculaCommandHandler 
3.Domain/Matricula -> registra evento de dominio
4.CrearMatriculaDomainEventHandler -> registra evento de integracion
5.RabbitMQEventBus


(primero creo matricula(con estado pendiente) y este siempre registrara un evento de integracion)
1. quien va estar publicando los eventos de integracion(Application/Matricula/CrearMatriculaDomainEventHandler)
RabbitMQEventBus: publica los eventos de integracion en RabbitMQ(registra en la cola de rabbit), los eventos a publicar son Application/Matricula/CrearMatriculaDomainEventHandler(cuando llegue a su publish MatriculaCreatedIntegrationEvent disparara hacia RabbitMQEventBus para publicarlo): eventName: MatriculaCreatedIntegrationEvent, message: es el record MatriculaCreatedIntegrationEven  Guid MatriculaId, Guid CursoId, body: lo mismo que el message en formato json, posteriormente lo publica al canal con BasicPublish

*********** FIN  FLUJO CREAR MATRICULA **************

(aqui se estara escuchando activamente si existen eventos de integracion)
2. quien va estar escuchando activamente las colas o las publicaciones (ExternalEvents/CursoConCupo(actualiza matricula confirmada)/CursoSinCupo(actualiza matricula rechazada))
RabbitMQEventListener: clase que estara escuchando activamente si existen colas, que cuando encuentre un evento o mensaje en la cola ejecutara la logica que tiene implementada


(lo que hara RabbitMQEventListener es disparar hacia uno de los handlers de cursoscupos y se ejecutara la logica que tenga)
3. una vez se escuchen las colas registradas de CursoSinCupoDisponibleIntegrationEvent o CursoConCupoDisponibleIntegrationEvent, lo que se hara es hacer un publish con MediatR y este disparara y se ejecutaran los handler de ExternalEvents/CursoSinCupo/CursoConCupo



(actualiza el estado de la matricula ya sea confirmada, rechazada, error)
4. ms. cursos vera si hay mensajes en las colas sobre matriculas creadas y  hara que se dispare los eventos de integracion de curso con cupo o sin cupo en el ms. de estudiantes para asi actualizar los estados de la matricula, pero antes de actualizer los estados de la matricula en ms.estudiantes, Habra una condicion en MatriculaCreatedIntegrationEventHandler donde si hay cupo disponible le resto un cupo y luego hago el evento de integracion de cursoConCupoDisponible(ms.Estudiantes) o si no hay cupo disponible entonces lo envio a CursoSinCupoDisponible(ambos eventos como lo mencione se ejecutaran en estudiantes)


*********** FIN  FLUJO CREAR CURSO **************

POSTMAN (ESCENARIO SIN COMPESACION) FLUJO IDEAL SIN ERROR(se registra matricula, matricula registra evento para curso, curso registra el evento para matricula, matricula lo lee y actualiza el estado a 1  )


1. CrearUsuario
2. ObtenerUsuario -> response newguid de usuario
3. ObtenerCurso -> response idcurso
4. CrearDocenteSimple -> request: especialidadid = response.idcurso y idusuario(de obtenerUsuario) | response: guid de docente
5. Estudiantes/CrearProgramacion -> request: cursoid y docenteid | response: guid de la programacion
6. Estudiantes/CrearEstudiante -> request: idusuario | response: guid de estudiante
7. Estudiantes/CrearMatricula -> request: estudianteid y programacionid | response: guid de matricula
en bd estudiantes -> select * from public.matricula (el estado debe cambiar de 0 a 1(aprobada))
8. ObtenerCurso -> en el response capacidadCurso deberia de reducirse






