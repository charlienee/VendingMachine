using System.Collections.Generic;
using System.Linq;
using System;

namespace VendingMachine;

public class TextFilter<T>
{
    private readonly Func<T, string> _selector;
    public TextFilter(Func<T, string> selector)
    {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
    }
    public IEnumerable<T> Apply(IEnumerable<T> source, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return source;
        return source.Where(item => _selector(item).Contains(searchText, StringComparison.OrdinalIgnoreCase));
    }
}