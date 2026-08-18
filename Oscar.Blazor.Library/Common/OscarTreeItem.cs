using MudBlazor;

namespace Oscar.Blazor.Library.Common
{
    /// <summary>
    /// Adapts an Oscar DTO node to the tree contract MudBlazor requires from v7 onwards.
    /// The wrapped DTO stays the source of truth for checked and expanded state, so the
    /// wrappers can be rebuilt freely on each render.
    /// </summary>
    public sealed class OscarTreeItem<T> : ITreeItemData<T> where T : class
    {
        public OscarTreeItem(T value, string text, IReadOnlyCollection<ITreeItemData<T>> children)
        {
            Value = value;
            Text = text;
            Children = children;
            Expandable = children.Count > 0;
        }

        public T? Value { get; set; }
        public string? Text { get; set; }
        public string? Icon { get; set; }
        public bool Expanded { get; set; }
        public bool Expandable { get; set; }
        public bool Selected { get; set; }
        public bool Visible { get; set; } = true;
        public IReadOnlyCollection<ITreeItemData<T>>? Children { get; set; }
        public bool HasChildren => Children is { Count: > 0 };

        /// <summary>
        /// Projects a DTO hierarchy into the wrapper collection MudTreeView binds to.
        /// </summary>
        public static List<ITreeItemData<T>> From(
            IEnumerable<T>? nodes,
            Func<T, IEnumerable<T>?> childrenOf,
            Func<T, string> textOf)
        {
            if (nodes is null)
                return new List<ITreeItemData<T>>();

            return nodes
                .Select(node => (ITreeItemData<T>)new OscarTreeItem<T>(
                    node,
                    textOf(node),
                    From(childrenOf(node), childrenOf, textOf)))
                .ToList();
        }
    }
}
