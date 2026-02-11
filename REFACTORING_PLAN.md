# План рефакторинга PowerGridEditor

## Ключевые проблемы

1. `Form1` совмещает UI, бизнес-правила, работу с файлами, импорт/экспорт и обработку графики. Файл разросся до ~1500+ строк, что усложняет поддержку и тестирование.
2. Во многих местах используются `object` и проверки `is`, из-за чего отсутствует типобезопасность и дублируются проверки типа.
3. Есть дублирование логики между `GraphicNode` и `GraphicBaseNode` (положение, отрисовка, выделение, попадание курсора).
4. В коде много `MessageBox.Show` и `Console.WriteLine` прямо внутри обработчиков, из-за чего смешаны ответственность UI и логика валидации/диагностики.
5. Логика валидации/поиска сущностей повторяется в добавлении и редактировании ветвей/шунтов.

## Целевая архитектура (без смены WinForms)

### Шаг 1: Ввести общие интерфейсы графических сущностей
- Добавить интерфейс `IGraphicNode`:
  - `int Number { get; }`
  - `Point Location { get; set; }`
  - `bool IsSelected { get; set; }`
  - `Point GetCenter()`
  - `bool Contains(Point p)`
  - `void Draw(Graphics g)`
- Перевести `GraphicNode` и `GraphicBaseNode` на этот интерфейс.
- Заменить `List<object> graphicElements` на `List<IGraphicNode>`.

**Эффект:** уйдут проверки типов `is GraphicNode / is GraphicBaseNode`, станет проще код выбора, перемещения и поиска узлов.

### Шаг 2: Разделить `Form1` на сервисы
Вынести из формы отдельные сервисы:
- `GridRepository` — хранение узлов/ветвей/шунтов и базовые CRUD-операции.
- `GridValidationService` — проверки уникальности номера узла, существования ветви/шунта, корректности ссылок.
- `GridFileService` — импорт/экспорт в файл (включая парсинг).
- `GridRenderService` — порядок отрисовки и вспомогательные методы вычисления координат.

**Эффект:** `Form1` останется оркестратором UI-событий, а не «монолитом».

### Шаг 3: Устранить дубли в сценариях Add/Edit
Сделать общие методы:
- `TryResolveNode(int number, out IGraphicNode node, out string error)`
- `ValidateBranchEndpoints(int start, int end, GraphicBranch current = null)`
- `ValidateShuntHost(int nodeNumber, GraphicShunt current = null)`

**Эффект:** единые правила валидации без копипаста и расхождений поведения.

### Шаг 4: Ввести типизированные связи
- В `GraphicBranch` заменить `object StartNode/EndNode` на `IGraphicNode`.
- В `GraphicShunt` заменить `object ConnectedNode` на `IGraphicNode`.

**Эффект:** компилятор начинает ловить ошибки на этапе сборки.

### Шаг 5: Нормализовать модели домена
- Вынести общие поля `Node` и `BaseNode` в общий базовый класс `NodeBaseData` (или интерфейс `INodeData`).
- Для строкового экспорта использовать единый форматтер с `CultureInfo.InvariantCulture`.

**Эффект:** меньше дублирования моделей и более надежный обмен файлами.

### Шаг 6: Улучшить тестируемость
- Добавить отдельный проект тестов (`PowerGridEditor.Tests`) хотя бы для:
  - валидации уникальности узлов,
  - проверки невозможности дублирующих ветвей,
  - парсинга строк импорта,
  - вычисления попадания на ветвь/шунт.

**Эффект:** безопасные изменения в будущем и меньше регрессий.

## Приоритизированный бэклог (что делать сначала)

1. **Быстрый выигрыш (1–2 дня):**
   - Ввести `IGraphicNode`.
   - Убрать `List<object>`/`object` в графических сущностях.
   - Вынести общие методы поиска/валидации узлов.
2. **Среднесрочно (2–4 дня):**
   - Разделить импорт/экспорт в `GridFileService`.
   - Вынести проверки в `GridValidationService`.
3. **Дальше (поэтапно):**
   - Упростить `Form1` до UI-оркестратора.
   - Добавить тесты.

## Минимальный пример первого шага

```csharp
public interface IGraphicNode
{
    int Number { get; }
    Point Location { get; set; }
    bool IsSelected { get; set; }
    Point GetCenter();
    bool Contains(Point point);
    void Draw(Graphics g);
}
```

После этого можно заменить `FindNodeByNumber` на типобезопасный поиск по `List<IGraphicNode>` и удалить значительную часть `if (x is GraphicNode ...) else if (x is GraphicBaseNode ...)`.
