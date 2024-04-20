using Microsoft.EntityFrameworkCore;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore;

public interface IContextProvider
{
    DbContext Context();
}