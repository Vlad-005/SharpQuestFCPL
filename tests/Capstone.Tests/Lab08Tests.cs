using Capstone.Core.Models;
using Capstone.Core.Wiring;

namespace Capstone.Tests;

public class Lab08Tests
{
    private readonly Lab08Wiring _wiring = new();

    [Fact]
    public async Task Missing_file_loads_as_a_working_empty_inventory()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.json");

        var loaded = await _wiring.LoadAsync(path);

        Assert.Empty(loaded.All());
        var exception = Record.Exception(() =>
            loaded.Add(new Consumable("never", "Never")));
        Assert.Null(exception);
    }

    [Fact]
    public async Task Save_and_load_preserve_order_kinds_values_and_quantities()
    {
        var path = NewTempPath();
        var inventory = new Inventory();
        inventory.AddToStack(new Weapon("w-1", "Iron Sword", 3), 2);
        inventory.AddToStack(new Armor("a-1", "Chain Mail", 5), 4);
        inventory.AddToStack(new Consumable("c-1", "Healing Potion", 1), 7);

        try
        {
            await _wiring.SaveAsync(inventory, path);
            var loaded = await _wiring.LoadAsync(path);

            Assert.Equal(inventory.All().Select(item => item.Id), loaded.All().Select(item => item.Id));
            Assert.Equal(inventory.Stacks.Select(stack => stack.Quantity),
                ((Inventory)loaded).Stacks.Select(stack => stack.Quantity));

            foreach (var saved in inventory.All())
            {
                var restored = loaded.FindById(saved.Id);
                Assert.NotNull(restored);
                Assert.Equal(saved.GetType(), restored!.GetType());
                Assert.Equal(saved.DisplayName, restored.DisplayName);
            }
        }
        finally
        {
            DeleteTempFolder(path);
        }
    }

    [Fact]
    public async Task Save_creates_missing_folders_and_replaces_the_file()
    {
        var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var path = Path.Combine(folder, "nested", "inventory.json");
        var inventory = new Inventory();
        inventory.Add(_wiring.CreateSample(1));

        try
        {
            await _wiring.SaveAsync(inventory, path);
            Assert.True(File.Exists(path));

            await _wiring.SaveAsync(new Inventory(), path);
            var loaded = await _wiring.LoadAsync(path);
            Assert.Empty(loaded.All());
        }
        finally
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public async Task Corrupt_or_wrong_shape_json_is_rejected()
    {
        var path = NewTempPath();
        try
        {
            await File.WriteAllTextAsync(path, "{ this is not json");
            await Assert.ThrowsAsync<InvalidDataException>(() => _wiring.LoadAsync(path));

            await File.WriteAllTextAsync(path, """{ "hello": "world" }""");
            await Assert.ThrowsAsync<InvalidDataException>(() => _wiring.LoadAsync(path));
        }
        finally
        {
            DeleteTempFolder(path);
        }
    }

    [Fact]
    public async Task Saving_is_a_snapshot()
    {
        var path = NewTempPath();
        var inventory = new Inventory();
        inventory.Add(_wiring.CreateSample(1));

        try
        {
            await _wiring.SaveAsync(inventory, path);
            inventory.Add(_wiring.CreateSample(2));

            var loaded = await _wiring.LoadAsync(path);
            Assert.Single(loaded.All());
        }
        finally
        {
            DeleteTempFolder(path);
        }
    }

    [Fact]
    public async Task Load_guards_and_save_guards_are_enforced()
    {
        var inventory = new Inventory();
        var path = NewTempPath();
        try
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _wiring.SaveAsync(null!, path));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _wiring.SaveAsync(inventory, null!));
            await Assert.ThrowsAsync<ArgumentException>(() => _wiring.SaveAsync(inventory, " "));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _wiring.LoadAsync(null!));
            await Assert.ThrowsAsync<ArgumentException>(() => _wiring.LoadAsync(" "));
        }
        finally
        {
            DeleteTempFolder(path);
        }
    }

    private static string NewTempPath()
    {
        var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "inventory.json");
    }

    private static void DeleteTempFolder(string path)
    {
        var folder = Path.GetDirectoryName(path);
        if (folder is not null && Directory.Exists(folder))
            Directory.Delete(folder, recursive: true);
    }
}
