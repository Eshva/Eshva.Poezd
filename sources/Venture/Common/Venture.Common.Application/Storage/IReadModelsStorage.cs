namespace Venture.Common.Application.Storage
{
  /// <summary>
  /// Контракт хранилища состояния продукта для данного сервиса. В этом хранилище прихраниваются данные,
  /// которые необходимы коду уровня приложение для принятия решений.
  /// </summary>
  /// <remarks>
  /// Например, предварительному условию выполнения use case необходимо удостовериться, что агрегат с таким ID уже не создан.
  /// Это можно реализовать либо обращением к хранилищу агрегатов (моделей записи), либо прикопав все ID этих агрегатов в
  /// hash map Redis и выполняя запрос к этому хранилищу моделей чтения в Redis. Запросы должны использовать это хранилище.
  /// </remarks>
  public interface IReadModelsStorage
  {
  }
}
