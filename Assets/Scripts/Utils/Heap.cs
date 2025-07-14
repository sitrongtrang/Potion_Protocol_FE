using System;

public class Heap<T> where T : IHeapItem<T> {
    private T[] _items;
    private int _currentItemCount;

    public Heap(int maxSize) => _items = new T[maxSize];
    public int Count => _currentItemCount;

    public void Add(T item) {
        item.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item;
        SortUp(item);
        _currentItemCount++;
    }

    public T RemoveFirst() {
        T firstItem = _items[0];
        _currentItemCount--;
        _items[0] = _items[_currentItemCount];
        _items[0].HeapIndex = 0;
        SortDown(_items[0]);
        return firstItem;
    }

    public void UpdateItem(T item) => SortUp(item);
    public bool Contains(T item) => Equals(_items[item.HeapIndex], item);

    private void SortUp(T item) {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true) {
            T parentItem = _items[parentIndex];
            if (item.CompareTo(parentItem) > 0) {
                Swap(item, parentItem);
            } else {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void SortDown(T item) {
        while (true) {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < _currentItemCount) {
                swapIndex = childIndexLeft;

                if (childIndexRight < _currentItemCount && 
                    _items[childIndexRight].CompareTo(_items[childIndexLeft]) > 0) {
                    swapIndex = childIndexRight;
                }

                if (item.CompareTo(_items[swapIndex]) < 0) {
                    Swap(item, _items[swapIndex]);
                } else {
                    return;
                }
            } else {
                return;
            }
        }
    }

    private void Swap(T a, T b) {
        _items[a.HeapIndex] = b;
        _items[b.HeapIndex] = a;
        (a.HeapIndex, b.HeapIndex) = (b.HeapIndex, a.HeapIndex);
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex { get; set; }
}