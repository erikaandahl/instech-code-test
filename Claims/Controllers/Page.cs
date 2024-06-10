namespace Claims.Controllers;

/// <summary>
///  Paging value object where first page is 1
/// </summary>
public struct Page
{
    public int CurrentPage { get; }
    public int PageSize { get; }
    public Page(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Invalid page size");
        CurrentPage = page;
        PageSize = pageSize;
    }
    public int Skip
    {
        get
        {
            return (CurrentPage - 1) * PageSize;
        }
    }

    public void VerifyPageSize(int max)
    {
        if (PageSize < 1 || PageSize > max)
            throw new ArgumentException($"Invalid page size, must be between 1 and {max}");
    }

    public static Page FirstPage(int pageSize)
    {
        return new Page(1, pageSize);
    }
        
    public static Page FromZeroBased(int page, int pageSize)
    {
        return new Page(page + 1, pageSize);
    }

    #region Equality Members
    public bool Equals(Page other)
    {
        return CurrentPage == other.CurrentPage && PageSize == other.PageSize;
    }
        
    public override bool Equals(object obj)
    {
        return obj is Page other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (CurrentPage * 397) ^ PageSize;
        }
    }
    #endregion
}