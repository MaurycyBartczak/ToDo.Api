# ToDo.Api

Prosta aplikacja API do zarządzania zadaniami (ToDo) napisana w .NET 9.0.

## Technologie

Aplikacja wykorzystuje:
- .NET 9.0 z Minimal API
- Entity Framework Core
- MySQL jako bazę danych
- xUnit do testów jednostkowych i integracyjnych


## Uruchomienie aplikacji

### Opcja 1: Uruchomienie z Docker Compose (zalecane)

1. Upewnij się, że masz zainstalowany Docker i Docker Compose
2. Otwórz terminal w głównym katalogu projektu (gdzie znajduje się plik `docker-compose.yml`)
3. Uruchom:

```bash
docker-compose up -d
```

Aplikacja będzie dostępna pod adresem: http://localhost:80/api


## Uruchamianie testów

```bash
dotnet test
```


## Dostępne endpointy API

- `GET /api/todos` - Pobierz wszystkie zadania
- `GET /api/todos/{id}` - Pobierz zadanie po ID
- `GET /api/todos/incoming/{timeFrame}` - Pobierz nadchodzące zadania (timeFrame: today, tomorrow, week)
- `POST /api/todos` - Utwórz nowe zadanie
- `PUT /api/todos/{id}` - Zaktualizuj zadanie
- `PATCH /api/todos/{id}/percent/{percent}` - Ustaw procent ukończenia zadania (0-100)
- `PATCH /api/todos/{id}/done` - Oznacz zadanie jako wykonane
- `DELETE /api/todos/{id}` - Usuń zadanie

