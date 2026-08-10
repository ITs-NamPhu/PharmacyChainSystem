using PharmacyManagement.DTOs.Medicine;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class MedicineMapper
    {
        public static Models.Medicine ToEntity(this CreateMedicineRequest request)
        {
            return new Models.Medicine
            {
                MedicineName = request.MedicineName,
                DefaultRetailPrice = request.DefaultRetailPrice,
                DefaultWholesalePrice = request.DefaultWholesalePrice,
                VATPercent = request.VATPercent,
                CategoryID = request.CategoryID,
                ManufacturerID = request.ManufacturerID,
                BaseUnitID = request.BaseUnitID
            };
        }

        public static void ApplyTo(this UpdateMedicineRequest request, Models.Medicine entity)
        {
            entity.MedicineName = request.MedicineName;
            entity.DefaultRetailPrice = request.DefaultRetailPrice;
            entity.DefaultWholesalePrice = request.DefaultWholesalePrice;
            entity.VATPercent = request.VATPercent;
            if (request.CategoryID.HasValue)
                entity.CategoryID = request.CategoryID.Value;
            if (request.ManufacturerID.HasValue)
                entity.ManufacturerID = request.ManufacturerID.Value;
            if (request.BaseUnitID.HasValue)
                entity.BaseUnitID = request.BaseUnitID.Value;
        }

        public static MedicineResponse ToResponse(this Models.Medicine entity)
        {
            return new MedicineResponse
            {
                MedicineID = entity.MedicineID,
                MedicineName = entity.MedicineName,
                DefaultRetailPrice = entity.DefaultRetailPrice,
                DefaultWholesalePrice = entity.DefaultWholesalePrice,
                VATPercent = entity.VATPercent,
                CategoryID = entity.CategoryID,
                CategoryName = entity.MedicineCategory?.CategoryName,
                ManufacturerID = entity.ManufacturerID,
                ManufacturerName = entity.ManuFacturer?.ManufacturerName,
                BaseUnitID = entity.BaseUnitID,
                UnitName = entity.Unit?.UnitName
            };
        }

        public static List<MedicineResponse> ToResponseList(this List<Models.Medicine> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
