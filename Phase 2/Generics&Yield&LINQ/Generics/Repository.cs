// Task 2.11 - Generic repository with constraints. (Seeds the Phase-7 data layer.)
namespace GenericsYieldLinq.Generics
{
    // Every entity the repository stores must expose an Id.
    public interface IEntity
    {
        int Id { get; set; }
    }

    /*
     * where T : class, IEntity, new()
     * -----------------------------------------------------------------------
     *  class    -> T must be a REFERENCE type. That lets GetById return null
     *              for "not found" (T? means "nullable reference").
     *              new Repository<int>()  -> ERROR CS0452 (int is a value type)
     *  IEntity  -> every T has an Id, so Update/Delete can locate items.
     *  new()    -> T has a public parameterless constructor, so the repository
     *              itself can create instances (see CreateNew).
     *              A class without one -> ERROR CS0310
     *  Order rule: 'class' comes first and 'new()' must be last.
     */
    public class Repository<T> where T : class, IEntity, new()
    {
        private readonly List<T> _items = new();

        public int Count => _items.Count;

        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (_items.Any(existing => existing.Id == item.Id))
                throw new InvalidOperationException($"{typeof(T).Name} with Id {item.Id} already exists.");
            _items.Add(item);
        }

        public bool Update(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            int index = _items.FindIndex(existing => existing.Id == item.Id);
            if (index < 0)
                return false;
            _items[index] = item;
            return true;
        }

        public bool Delete(int id) => _items.RemoveAll(item => item.Id == id) > 0;

        public T? GetById(int id) => _items.FirstOrDefault(item => item.Id == id);

        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

        // Only possible because of the new() constraint.
        public T CreateNew(int id) => new T { Id = id };
    }
}
