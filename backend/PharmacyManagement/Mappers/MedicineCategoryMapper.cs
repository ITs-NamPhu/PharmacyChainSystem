using PharmacyManagement.DTOs.MedicineCategory;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class MedicineCategoryMapper
    {
        public static MedicineCategory ToEntity(this CreateMedicineCategoryRequest request)
        {
            return new MedicineCategory
            {
                CategoryName = request.CategoryName
            };
        }

        public static void ApplyTo(this UpdateMedicineCategoryRequest request, MedicineCategory entity)
        {
            entity.CategoryName = request.CategoryName;
        }

        public static MedicineCategoryResponse ToResponse(this MedicineCategory entity)
        {
            return new MedicineCategoryResponse
            {
                MedicineCategoryID = entity.MedicineCategoryID,
                CategoryName = entity.CategoryName
            };
        }

        public static List<MedicineCategoryResponse> ToResponseList(this List<MedicineCategory> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
