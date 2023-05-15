using System.Linq.Expressions;
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface ICompanyScreenerService
{
    Task<ICollection<Company>> ScreenAsync();
}