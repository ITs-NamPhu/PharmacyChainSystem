using PharmacyManagement.DTOs.Unit;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class UnitMapper
    {
        public static Unit ToEntity(this CreateUnitRequest request)
        {
            return new Unit
            {
                UnitName = request.UnitName
            };
        }

        public static void ApplyTo(this UpdateUnitRequest request, Unit entity)
        {
            entity.UnitName = request.UnitName;
        }

        public static UnitResponse ToResponse(this Unit entity)
        {
            return new UnitResponse
            {
                UnitID = entity.UnitID,
                UnitName = entity.UnitName
            };
        }

        public static List<UnitResponse> ToResponseList(this List<Unit> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
