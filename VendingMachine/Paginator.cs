using System.Collections.Generic;
using System.Linq;

namespace VendingMachine;

public class Paginator<T>
{
    public int PageSize {get; set;} 
    public int CurrentPage {get; private set;} = 1;
    public Paginator(int pageSize)
    {
        PageSize = pageSize;
    }
    public IEnumerable<T> Apply (IEnumerable<T> source)
    {
        return source.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
    }
    public bool CanMoveNext(int totalCount)
    {
        return CurrentPage * PageSize < totalCount;
    }
    public void MoveNext()
    {
        CurrentPage ++;
    }
    public void MovePrevious()
    {
        if (CurrentPage > 1)
            CurrentPage--;
    }
    public void Reset()
    {
        CurrentPage = 1;
    }
}