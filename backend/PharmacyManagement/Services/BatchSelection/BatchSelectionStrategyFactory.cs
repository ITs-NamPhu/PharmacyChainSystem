using PharmacyManagement.DTOs.Invoice;

namespace PharmacyManagement.Services.BatchSelection
{
    public class BatchSelectionStrategyFactory
    {
        private readonly FefoBatchSelectionStrategy _fefoStrategy;
        private readonly ManualBatchSelectionStrategy _manualStrategy;

        public BatchSelectionStrategyFactory(
            FefoBatchSelectionStrategy fefoStrategy,
            ManualBatchSelectionStrategy manualStrategy)
        {
            _fefoStrategy = fefoStrategy;
            _manualStrategy = manualStrategy;
        }

        public IBatchSelectionStrategy GetStrategy(BatchSelectionMode mode)
        {
            return mode switch
            {
                BatchSelectionMode.FEFO => _fefoStrategy,
                BatchSelectionMode.Manual => _manualStrategy,
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };
        }
    }
}
