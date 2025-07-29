using LiquidLapsAPI.Models;

namespace LiquidLapsAPI.Services
{
    public interface ILiquidLabsService
    {
        Task<List<LiquidLabsModel>> GetAll();
        Task<LiquidLabsModel> GetById(int id);
    }
}
