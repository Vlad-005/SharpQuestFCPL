using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.Core.ViewModels;

public sealed class CatalogueViewModel : ICatalogueViewModel
{
    private readonly IItemStore _store;
    private readonly Inventory? _inventory;
    private readonly ObservableCollection<IItem> _items = [];
    private string _filter = "";
    private IItem? _selected;

    public CatalogueViewModel(IItemStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        _store = store;
        _inventory = store as Inventory;
        if (_inventory is not null)
            _inventory.ItemAdded += OnItemAdded;

        foreach (var item in store.All())
            _items.Add(item);

        AddCommand = new RelayCommand(item =>
        {
            if (item is IItem inventoryItem)
            {
                _store.Add(inventoryItem);
            }
        });

        RemoveCommand = new RelayCommand(
            _ => RemoveSelected(),
            _ => _selected is not null);
    }

    public IEnumerable<IItem> Items => _items;

    public string Filter
    {
        get => _filter;
        set
        {
            var normalized = value?.Trim() ?? "";
            if (string.Equals(_filter, normalized, StringComparison.Ordinal))
                return;

            _filter = normalized;
            RebuildVisibleItems();
            RaisePropertyChanged(nameof(Filter));
        }
    }

    public IItem? Selected
    {
        get => _selected;
        set
        {
            if (ReferenceEquals(_selected, value))
                return;

            _selected = value;
            RaisePropertyChanged(nameof(Selected));
            RaisePropertyChanged(nameof(Details));
            ((RelayCommand)RemoveCommand).RaiseCanExecuteChanged();
        }
    }

    public string Details => _selected switch
    {
        null => "Nothing selected.",
        Item item => $"Id: {item.Id} | Name: {item.DisplayName} | Category: {item.Category} | Required level: {item.RequiredLevel}",
        IItem item => $"Id: {item.Id} | Name: {item.DisplayName}"
    };

    public int Count => _items.Count;

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnItemAdded(IItem item)
    {
        if (!MatchesFilter(item))
            return;

        _items.Add(item);
        RaisePropertyChanged(nameof(Count));
    }

    private void RemoveSelected()
    {
        if (_selected is null)
            return;

        var item = _selected;
        if (!_store.Remove(item.Id))
            return;

        _items.Remove(item);
        Selected = null;
        RaisePropertyChanged(nameof(Count));
    }

    private bool MatchesFilter(IItem item)
        => string.IsNullOrWhiteSpace(_filter)
           || item.DisplayName.Contains(_filter, StringComparison.OrdinalIgnoreCase);

    private void RebuildVisibleItems()
    {
        _items.Clear();
        foreach (var item in _store.All())
            if (MatchesFilter(item))
                _items.Add(item);

        if (_selected is not null && _store.FindById(_selected.Id) is null)
            Selected = null;

        RaisePropertyChanged(nameof(Count));
    }

    private void RaisePropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private sealed class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        : ICommand
    {
        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => execute(parameter);

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
