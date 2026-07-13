namespace WebApp.Api.Services;

/// <summary>
/// Интерфейс утилиты.
/// Каждая утилита реализует Endpoint (уникальный ключ) и Execute (бизнес-логика).
/// </summary>
public interface IUtilityService
{
    /// <summary>Уникальный идентификатор утилиты (используется в URL)</summary>
    string Endpoint { get; }

    /// <summary>Бизнес-логика: принимает строку ввода, возвращает строку результата</summary>
    string Execute(string input);
}
