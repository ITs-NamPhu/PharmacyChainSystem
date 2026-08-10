using PharmacyManagement.DTOs.UnitConversion;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class UnitConversionMapper
    {
        public static UnitConversion ToEntity(this CreateUnitConversionRequest request)
        {
            return new UnitConversion
            {
                UnitID = request.UnitID,
                MedicineID = request.MedicineID,
                Factor = request.Factor
            };
        }

        public static void ApplyTo(this UpdateUnitConversionRequest request, UnitConversion entity)
        {
            entity.Factor = request.Factor;
        }

        public static UnitConversionResponse ToResponse(this UnitConversion entity)
        {
            return new UnitConversionResponse
            {
                UnitConversionID = entity.UnitConversionID,
                UnitID = entity.UnitID,
                MedicineID = entity.MedicineID,
                Factor = entity.Factor
            };
        }

        public static UnitConversion_MedicineResponse ToResponse_MedicineName(this UnitConversion entity)
        {
            return new UnitConversion_MedicineResponse
            {
                UnitConversionID = entity.UnitConversionID,
                UnitID = entity.UnitID,
                MedicineID = entity.MedicineID,
                Factor = entity.Factor,
                MedicineName = entity.Medicine.MedicineName
            };
        }
        public static List<UnitConversionResponse> ToResponseList(this List<UnitConversion> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
