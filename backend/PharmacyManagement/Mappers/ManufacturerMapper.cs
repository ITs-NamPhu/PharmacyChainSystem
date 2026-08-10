using PharmacyManagement.DTOs.Manufacturer;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class ManufacturerMapper
    {
        public static ManuFacturer ToEntity(this CreateManufacturerRequest request)
        {
            return new ManuFacturer
            {
                ManufacturerName = request.ManufacturerName
            };
        }

        public static void ApplyTo(this UpdateManufacturerRequest request, ManuFacturer entity)
        {
            entity.ManufacturerName = request.ManufacturerName;
        }

        public static ManufacturerResponse ToResponse(this ManuFacturer entity)
        {
            return new ManufacturerResponse
            {
                ManufacturerID = entity.ManufacturerID,
                ManufacturerName = entity.ManufacturerName
            };
        }

        public static List<ManufacturerResponse> ToResponseList(this List<ManuFacturer> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
