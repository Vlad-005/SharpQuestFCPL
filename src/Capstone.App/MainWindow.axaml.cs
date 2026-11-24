using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Capstone.Core.Models;
using SharpQuest.Contracts;

namespace Capstone.App;

public sealed partial class MainWindow : Window
{
    private readonly Inventory _inventory = new();
    private readonly ObservableCollection<ChestEntry> _chest = [];

    public MainWindow()
    {
        InitializeComponent();

        _inventory.Add(new Weapon("sword-1", "Iron Sword", 3));
        _inventory.Add(new Armor("armor-1", "Leather Armor", 2));
        _inventory.Add(new Consumable("potion-1", "Healing Potion", 1));

        InventoryList.ItemsSource = _inventory.Stacks
            .Select(stack => new InventoryEntry(stack.Item, stack.Quantity))
            .ToArray();
        ChestDepositList.ItemsSource = InventoryList.ItemsSource;
        ChestList.ItemsSource = _chest;
        RefreshChestCount();

        InventoryButton.Click += (_, _) =>
        {
            InventoryPanel.IsVisible = !InventoryPanel.IsVisible;
            ChestPanel.IsVisible = false;
        };
        ChestButton.Click += (_, _) =>
        {
            ChestPanel.IsVisible = !ChestPanel.IsVisible;
            InventoryPanel.IsVisible = false;
        };
        CloseInventoryButton.Click += (_, _) => InventoryPanel.IsVisible = false;
        CloseChestButton.Click += (_, _) => ChestPanel.IsVisible = false;
        DepositButton.Click += DepositClicked;
        WithdrawButton.Click += WithdrawClicked;
    }

    private void DepositClicked(object? sender, RoutedEventArgs e)
    {
        if (ChestDepositList.SelectedItem is not InventoryEntry entry)
            return;

        if (!_inventory.RemoveFromStack(entry.Item.Id, 1))
            return;

        if (entry.Item is Item item)
            AddToChest(item);
        RefreshLists();
    }

    private void WithdrawClicked(object? sender, RoutedEventArgs e)
    {
        if (ChestList.SelectedItem is not ChestEntry chestEntry)
            return;

        if (chestEntry.Item is Item item)
            _inventory.AddToStack(item, 1);
        if (chestEntry.Quantity == 1)
            _chest.Remove(chestEntry);
        else
        {
            var index = _chest.IndexOf(chestEntry);
            _chest[index] = new ChestEntry(chestEntry.Item, chestEntry.Quantity - 1);
        }

        RefreshLists();
    }

    private void AddToChest(IItem item)
    {
        var existing = _chest.FirstOrDefault(entry =>
            string.Equals(entry.Item.Id, item.Id, StringComparison.Ordinal));

        if (existing is null)
        {
            _chest.Add(new ChestEntry(item, 1));
            return;
        }

        var index = _chest.IndexOf(existing);
        _chest[index] = new ChestEntry(existing.Item, existing.Quantity + 1);
    }

    private void RefreshLists()
    {
        var inventoryEntries = _inventory.Stacks
            .Select(stack => new InventoryEntry(stack.Item, stack.Quantity))
            .ToArray();

        InventoryList.ItemsSource = null;
        InventoryList.ItemsSource = inventoryEntries;
        ChestDepositList.ItemsSource = null;
        ChestDepositList.ItemsSource = inventoryEntries;
        ChestList.ItemsSource = null;
        ChestList.ItemsSource = _chest;

        var quantity = _chest.Sum(entry => entry.Quantity);
        ChestCountText.Text = quantity == 0
            ? "Chest is empty"
            : $"{quantity} item{(quantity == 1 ? "" : "s")} stored";
    }

    private void RefreshChestCount()
    {
        ChestCountText.Text = "Chest is empty";
    }

    private sealed record ChestEntry(IItem Item, int Quantity)
    {
        public override string ToString()
            => $"{Item.DisplayName} ({Quantity})";
    }

    private sealed record InventoryEntry(IItem Item, int Quantity)
    {
        public override string ToString()
            => $"{Item.DisplayName} ({Quantity})";
    }
}
